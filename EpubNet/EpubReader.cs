﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using EpubNet.Entities;
using EpubNet.Readers;
using EpubNet.RefEntities;

namespace EpubNet
{
	public static class EpubReader
	{
		#region Public API

		/// <summary>
		/// 	Opens the book asynchronously and reads all of its content into the memory. Does not hold the handle to the EPUB file.
		/// </summary>
		/// <param name="stream">Stream of EPUB book</param>
		/// <returns><seealso cref="EpubBook"/></returns>
		public static async Task<EpubBook> ReadBookAsync(Stream stream)
		{
			var result = new EpubBook();
			using (var epubBookRef = await OpenBookAsync(stream).ConfigureAwait(false))
			{
				result.Schema = epubBookRef.Schema;
				result.Title = epubBookRef.Title;
				result.AuthorList = epubBookRef.AuthorList;
				result.Content = await ReadContent(epubBookRef.Content).ConfigureAwait(false);
				result.CoverImage = await BookCoverReader.ReadBookCoverAsync(epubBookRef).ConfigureAwait(false);
				var chapterRefs = ChapterReader.GetChapters(epubBookRef);
				result.Chapters = await ReadChapters(chapterRefs).ConfigureAwait(false);
			}

			return result;
		}

		/// <summary>
		///     Opens the book asynchronously and reads all of its content into the memory. Does not hold the handle to the EPUB file.
		/// </summary>
		/// <param name="filePath">path to the EPUB file</param>
		/// <returns></returns>
		public static async Task<EpubBook> ReadBookAsync(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException("Specified epub file not found.", filePath);

			var result = await ReadBookAsync(new FileStream(filePath, FileMode.Open));
			result.FilePath = filePath;
			return result;
		}

		#endregion

		#region Non-public methods

		/// <summary>
		///     Opens the book asynchronously without reading its content. Holds the handle to the EPUB file.
		/// </summary>
		/// <param name="stream">Stream of file to be parsed</param>
		/// <returns></returns>
		private static async Task<EpubBookRef> OpenBookAsync(Stream stream)
		{
			var epubArchive = new ZipArchive(stream);
			var bookRef = new EpubBookRef(epubArchive) { Schema = await SchemaReader.ReadSchemaAsync(epubArchive).ConfigureAwait(false) };
			bookRef.Title = bookRef.Schema.Package.Metadata.Titles.FirstOrDefault() ?? string.Empty;
			bookRef.AuthorList = bookRef.Schema.Package.Metadata.Creators.Select(creator => creator.Creator).ToList();
			bookRef.Content = ContentReader.ParseContentMap(bookRef);
			return bookRef;
		}

		private static async Task<EpubContent> ReadContent(EpubContentRef contentRef)
		{
			var result = new EpubContent
			{
				Html = await ReadTextContentFiles(contentRef.Html).ConfigureAwait(false),
				Css = await ReadTextContentFiles(contentRef.Css).ConfigureAwait(false),
				Images = await ReadByteContentFiles(contentRef.Images).ConfigureAwait(false),
				Fonts = await ReadByteContentFiles(contentRef.Fonts).ConfigureAwait(false),
				AllFiles = new Dictionary<string, EpubContentFile>()
			};
			foreach (var textContentFile in result.Html.Concat(result.Css))
				result.AllFiles.Add(textContentFile.Key, textContentFile.Value);

			foreach (var byteContentFile in result.Images.Concat(result.Fonts))
				result.AllFiles.Add(byteContentFile.Key, byteContentFile.Value);

			foreach (var contentFileRef in contentRef.AllFiles)
				if (!result.AllFiles.ContainsKey(contentFileRef.Key))
					result.AllFiles.Add(contentFileRef.Key, await ReadByteContentFile(contentFileRef.Value).ConfigureAwait(false));

			return result;
		}

		private static async Task<Dictionary<string, EpubTextContentFile>> ReadTextContentFiles(Dictionary<string, EpubTextContentFileRef> textContentFileRefs)
		{
			var result = new Dictionary<string, EpubTextContentFile>();
			foreach (var textContentFileRef in textContentFileRefs)
			{
				var textContentFile = new EpubTextContentFile
				{
					FileName = textContentFileRef.Value.FileName,
					ContentType = textContentFileRef.Value.ContentType,
					ContentMimeType = textContentFileRef.Value.ContentMimeType,
					Content = await textContentFileRef.Value.ReadContentAsTextAsync().ConfigureAwait(false)
				};
				result.Add(textContentFileRef.Key, textContentFile);
			}

			return result;
		}

		private static async Task<Dictionary<string, EpubByteContentFile>> ReadByteContentFiles(Dictionary<string, EpubByteContentFileRef> byteContentFileRefs)
		{
			var result = new Dictionary<string, EpubByteContentFile>();
			foreach (var byteContentFileRef in byteContentFileRefs)
				result.Add(byteContentFileRef.Key, await ReadByteContentFile(byteContentFileRef.Value).ConfigureAwait(false));

			return result;
		}

		private static async Task<EpubByteContentFile> ReadByteContentFile(EpubContentFileRef contentFileRef)
		{
			var result = new EpubByteContentFile
			{
				FileName = contentFileRef.FileName,
				ContentType = contentFileRef.ContentType,
				ContentMimeType = contentFileRef.ContentMimeType,
				Content = await contentFileRef.ReadContentAsBytesAsync().ConfigureAwait(false)
			};
			return result;
		}

		private static async Task<List<EpubChapter>> ReadChapters(IEnumerable<EpubChapterRef> chapterRefs)
		{
			var result = new List<EpubChapter>();
			foreach (var chapterRef in chapterRefs)
			{
				var chapter = new EpubChapter
				{
					Title = chapterRef.Title,
					ContentFileName = chapterRef.ContentFileName,
					Anchor = chapterRef.Anchor,
					HtmlContent = await chapterRef.ReadHtmlContentAsync().ConfigureAwait(false),
					SubChapters = await ReadChapters(chapterRef.SubChapters).ConfigureAwait(false)
				};
				result.Add(chapter);
			}

			return result;
		}

		#endregion
	}
}