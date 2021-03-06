﻿using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubManifestItem
	{
		public string Id { get; set; }
		public string Href { get; set; }
		public string MediaType { get; set; }
		public string RequiredNamespace { get; set; }
		public string RequiredModules { get; set; }
		public string Fallback { get; set; }
		public string FallbackStyle { get; set; }

		public override string ToString() => $"Id: {Id}, Href = {Href}, MediaType = {MediaType}";
	}
}