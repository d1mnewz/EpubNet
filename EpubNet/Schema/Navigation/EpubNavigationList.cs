using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Navigation
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubNavigationList
	{
		public string Id { get; set; }
		public string Class { get; set; }
		public List<EpubNavigationLabel> NavigationLabels { get; set; }
		public List<EpubNavigationTarget> NavigationTargets { get; set; }
	}
}