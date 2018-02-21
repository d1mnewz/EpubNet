namespace EpubNet.Schema.Navigation
{
	public class EpubNavigationContent
	{
		public string Id { get; set; }
		public string Source { get; set; }

		public override string ToString() => $"Source: {Source}";
	}
}