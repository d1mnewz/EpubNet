using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Entities
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubByteContentFile : EpubContentFile
	{
		public byte[] Content { get; set; }
	}
}