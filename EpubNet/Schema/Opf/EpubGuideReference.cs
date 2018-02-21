namespace EpubNet.Schema.Opf
{
	public class EpubGuideReference
	{
		public string Type { get; set; }
		public string Title { get; set; }
		public string Href { get; set; }

		public override string ToString() => $"Type: {Type}, Href: {Href}";
		
	}
}