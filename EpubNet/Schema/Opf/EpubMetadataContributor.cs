using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubMetadataContributor
	{
		public string Contributor { get; set; }
		public string FileAs { get; set; }
		public string Role { get; set; }
	}
}