﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Navigation
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubNavigation
	{
		public EpubNavigationHead Head { get; set; }
		public EpubNavigationDocTitle DocTitle { get; set; }
		public List<EpubNavigationDocAuthor> DocAuthors { get; set; }
		public EpubNavigationMap NavMap { get; set; }
		public EpubNavigationPageList PageList { get; set; }
		public List<EpubNavigationList> NavLists { get; set; }
	}
}