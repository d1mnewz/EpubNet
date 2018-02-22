using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpubNet.RefEntities
{
	public class EpubChapterRef
	{
		private readonly EpubTextContentFileRef _epubTextContentFileRef;

		public EpubChapterRef(EpubTextContentFileRef epubTextContentFileRef) => _epubTextContentFileRef = epubTextContentFileRef;

		public string Title { get; set; }
		public string ContentFileName { get; set; }
		public string Anchor { get; set; }
		public List<EpubChapterRef> SubChapters { get; set; }

		public Task<string> ReadHtmlContentAsync() => _epubTextContentFileRef.ReadContentAsTextAsync();

		public override string ToString() => $"Title: {Title}, Subchapter count: {SubChapters.Count}";
	}
}