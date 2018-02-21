using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubMetadataIdentifier
	{
		public string Id { get; set; }
		public string Scheme { get; set; }
		public string Identifier { get; set; }
	}
}