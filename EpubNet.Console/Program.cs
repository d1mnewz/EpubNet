using System.IO;
using System.Threading.Tasks;

namespace EpubNet.Console
{
	internal static class Program
	{
		private static async Task Main()
		{
			const string filename = "1.epub";
			var bytes = File.ReadAllBytes(filename);
			var stream = new MemoryStream(bytes);
			var book = await EpubReader.ReadBookAsync(stream);

			System.Console.WriteLine(book.ToPlainText());
		}
	}
}