using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubPackage
	{
		public EpubVersion EpubVersion { get; set; }
		public EpubMetadata Metadata { get; set; }
		public EpubManifest Manifest { get; set; }
		public EpubSpine Spine { get; set; }
		public EpubGuide Guide { get; set; }
	}
}