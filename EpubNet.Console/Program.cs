using System.IO;
using System.Threading.Tasks;
using EpubNet.Console.Extensions;

namespace EpubNet.Console
{
	internal static class Program
	{
		private static async Task Main()
		{
			const string filename = "1.epub";
			var bytes = await File.ReadAllBytesAsync(filename);
			var stream = new MemoryStream(bytes);
			
			var book = await EpubReader.ReadBookAsync(bytes);

			var bookFromStream = await EpubReader.ReadBookAsync(stream);

			var bookFromFile = await EpubReader.ReadBookAsync(filename);

			System.Console.WriteLine(book.ToPlainText());
		}
	}
}