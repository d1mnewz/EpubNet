using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubGuideReference
	{
		public string Type { get; set; }
		public string Title { get; set; }
		public string Href { get; set; }

		public override string ToString()
		{
			return $"Type: {Type}, Href: {Href}";
		}
	}
}