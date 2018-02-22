using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using EpubNet.Extensions;

namespace EpubNet.Entities
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubBook
	{
		public string FilePath { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public List<string> AuthorList { get; set; }
		public EpubSchema Schema { get; set; }
		public EpubContent Content { get; set; }
		public byte[] CoverImage { get; set; }
		public List<EpubChapter> Chapters { get; set; }

		public string ContentAsPlainText => this.ToPlainText();

		public IEnumerable<string> ChaptersAsPlainTexts => Chapters.Select(x => x.ToPlainText()).ToList();

		public int TotalCharactersCount => ContentAsPlainText.Length;

		public int CharactersCountInChapters => ChaptersAsPlainTexts.Sum(x => x.Length);

		public int TotalWordsCount => Regex.Matches(ContentAsPlainText, WordsRegex).Count;

		public int WordsCountInChapters => Chapters.Select(x => x.ToPlainText()).Sum(x => Regex.Matches(x, WordsRegex).Count);

		private const string WordsRegex = @"\w+[^\s]*\w+|\w";
	}
}