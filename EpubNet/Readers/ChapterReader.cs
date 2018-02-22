using System;
using System.Collections.Generic;
using System.Linq;
using EpubNet.RefEntities;
using EpubNet.Schema.Navigation;

namespace EpubNet.Readers
{
	internal static class ChapterReader
	{
		public static IEnumerable<EpubChapterRef> GetChapters(EpubBookRef bookRef) => GetChapters(bookRef, bookRef.Schema.Navigation.NavMap);

		private static List<EpubChapterRef> GetChapters(EpubBookRef bookRef, IEnumerable<EpubNavigationPoint> navigationPoints)
		{
			var result = new List<EpubChapterRef>();
			foreach (var navigationPoint in navigationPoints)
			{
				string contentFileName;
				string anchor;
				var contentSourceAnchorCharIndex = navigationPoint.Content.Source.IndexOf('#');
				if (contentSourceAnchorCharIndex is -1)
				{
					contentFileName = navigationPoint.Content.Source;
					anchor = null;
				}
				else
				{
					contentFileName = navigationPoint.Content.Source.Substring(0, contentSourceAnchorCharIndex);
					anchor = navigationPoint.Content.Source.Substring(contentSourceAnchorCharIndex + 1);
				}

				if (!bookRef.Content.Html.TryGetValue(contentFileName, out var htmlContentFileRef))
					throw new Exception($"Incorrect EPUB manifest: item with href = \"{contentFileName}\" is missing.");

				var chapterRef = new EpubChapterRef(htmlContentFileRef)
				{
					ContentFileName = contentFileName,
					Anchor = anchor,
					Title = navigationPoint.NavigationLabels.First().Text,
					SubChapters = GetChapters(bookRef, navigationPoint.ChildNavigationPoints)
				};
				result.Add(chapterRef);
			}

			return result;
		}
	}
}