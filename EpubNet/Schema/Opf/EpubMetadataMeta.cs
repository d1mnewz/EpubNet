using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubMetadataMeta
	{
		public string Name { get; set; }
		public string Content { get; set; }
		public string Id { get; set; }
		public string Refines { get; set; }
		public string Property { get; set; }
		public string Scheme { get; set; }
	}
}