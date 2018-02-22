using System;
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
		public string Author => string.Join(", ", AuthorList);
		public List<string> AuthorList { get; set; }
		public EpubSchema Schema { get; set; }
		public EpubContent Content { get; set; }
		public byte[] CoverImage { get; set; }
		public List<EpubChapter> Chapters { get; set; }


		public string ContentAsPlainText => _contentAsPlainText ?? (_contentAsPlainText = this.ToPlainText());
		private string _contentAsPlainText;

		public IList<string> ChaptersAsPlainTexts => _chaptersAsPlainTexts ?? (_chaptersAsPlainTexts = Chapters.Select(x => x.ToPlainText()).ToList());
		private List<string> _chaptersAsPlainTexts;

		public int TotalCharactersCount => (_totalCharactersCount ?? (_totalCharactersCount = ContentAsPlainText.Length)).Value;
		private int? _totalCharactersCount;

		public int CharactersCountInChapters => (_charactersCountInChapters ?? (_charactersCountInChapters = ChaptersAsPlainTexts.Sum(x => x.Length))).Value;
		private int? _charactersCountInChapters;

		public int TotalWordsCount => (_totalWordsCount ?? (_totalWordsCount = Regex.Matches(ContentAsPlainText, WordsRegex).Count)).Value;
		private int? _totalWordsCount;

		public int WordsCountInChapters => (_wordsCountInChapters ??
		                                    (_wordsCountInChapters = Chapters.Select(x => x.ToPlainText()).Sum(x => Regex.Matches(x, WordsRegex).Count))).Value;

		private int? _wordsCountInChapters;

		private const string WordsRegex = @"\w+[^\s]*\w+|\w";
	}
}