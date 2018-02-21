using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Navigation
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubNavigationTarget
	{
		public string Id { get; set; }
		public string Class { get; set; }
		public string Value { get; set; }
		public string PlayOrder { get; set; }
		public List<EpubNavigationLabel> NavigationLabels { get; set; }
		public EpubNavigationContent Content { get; set; }
	}
}