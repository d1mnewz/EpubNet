using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using EpubNet.Entities;
using EpubNet.Readers;

namespace EpubNet.RefEntities
{
	public sealed class EpubBookRef : IDisposable
	{
		private bool _isDisposed;

		public EpubBookRef(ZipArchive epubArchive)
		{
			EpubArchive = epubArchive;
			_isDisposed = false;
		}

		public string FilePath { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public List<string> AuthorList { get; set; }
		public EpubSchema Schema { get; set; }
		public EpubContentRef Content { get; set; }

		internal ZipArchive EpubArchive { get; }


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~EpubBookRef()
		{
			Dispose(false);
		}

		public async Task<byte[]> ReadCover()
		{
			return await ReadCoverAsync();
		}

		public async Task<byte[]> ReadCoverAsync()
		{
			return await BookCoverReader.ReadBookCoverAsync(this).ConfigureAwait(false);
		}

		public IEnumerable<EpubChapterRef> GetChapters()
		{
			return ChapterReader.GetChapters(this);
		}

		private void Dispose(bool disposing)
		{
			if (_isDisposed) return;
			if (disposing) EpubArchive.Dispose();

			_isDisposed = true;
		}
	}
}