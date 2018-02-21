using System.Collections.Generic;

namespace EpubNet.Schema.Opf
{
	public class EpubSpine : List<EpubSpineItemRef>
	{
		public string Toc { get; set; }
	}
}