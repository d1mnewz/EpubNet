using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Navigation
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubNavigationContent
	{
		public string Id { get; set; }
		public string Source { get; set; }

		public override string ToString() => $"Source: {Source}";
	}
}