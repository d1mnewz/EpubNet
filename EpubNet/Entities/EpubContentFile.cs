using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Entities
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public abstract class EpubContentFile
	{
		public string FileName { get; set; }
		public EpubContentType ContentType { get; set; }
		public string ContentMimeType { get; set; }
	}
}