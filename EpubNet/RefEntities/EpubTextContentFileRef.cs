using System.Threading.Tasks;

namespace EpubNet.RefEntities
{
	public class EpubTextContentFileRef : EpubContentFileRef
	{
		public EpubTextContentFileRef(EpubBookRef epubBookRef)
			: base(epubBookRef)
		{
		}

		public Task<string> ReadContentAsync()
		{
			return ReadContentAsTextAsync();
		}
	}
}