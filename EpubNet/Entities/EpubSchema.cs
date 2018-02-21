using EpubNet.Schema.Navigation;
using EpubNet.Schema.Opf;

namespace EpubNet.Entities
{
	public class EpubSchema
	{
		public EpubPackage Package { get; set; }
		public EpubNavigation Navigation { get; set; }
		public string ContentDirectoryPath { get; set; }
	}
}