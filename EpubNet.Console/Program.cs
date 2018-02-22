using System.IO;
using System.Threading.Tasks;

namespace EpubNet.Console
{
	internal static class Program
	{
		private static async Task Main()
		{
			const string filename = "1.epub";
			var bytes = await File.ReadAllBytesAsync(filename);
			var stream = new MemoryStream(bytes);

			var book = await EpubReader.ReadBookAsync(stream);

			var bookFromStream = await EpubReader.ReadBookAsync(stream);

			var bookFromFile = await EpubReader.ReadBookAsync(filename);

			var contentAsPlainText = book.ContentAsPlainText;
			var chaptersAsPlainText = book.ChaptersAsPlainTexts;
			var totalCharactersCount = book.TotalCharactersCount;
			var totalWordsCount = book.TotalWordsCount;
			var wordsCountInChapters = book.WordsCountInChapters;
			var charactersCountInChapters = book.CharactersCountInChapters;
		}
	}
}