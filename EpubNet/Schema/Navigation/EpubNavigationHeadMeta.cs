using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Navigation
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubNavigationHeadMeta
	{
		public string Name { get; set; }
		public string Content { get; set; }
		public string Scheme { get; set; }
	}
}