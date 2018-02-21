using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Entities
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
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