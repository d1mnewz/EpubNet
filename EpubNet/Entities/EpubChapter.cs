using System.Collections.Generic;

namespace EpubNet.Entities
{
	public class EpubChapter
	{
		public string Title { get; set; }
		public string ContentFileName { get; set; }
		public string Anchor { get; set; }
		public string HtmlContent { get; set; }
		public List<EpubChapter> SubChapters { get; set; }

		public override string ToString()
		{
			return $"Title: {Title}, Subchapter count: {SubChapters.Count}";
		}
	}
}