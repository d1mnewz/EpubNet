using System;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using EpubNet.Utils;

namespace EpubNet.Readers
{
	internal static class RootFilePathReader
	{
		public static async Task<string> GetRootFilePathAsync(ZipArchive epubArchive)
		{
			const string epubContainerFilePath = "META-INF/container.xml";
			var containerFileEntry = epubArchive.GetEntry(epubContainerFilePath);
			if (containerFileEntry is null) throw new Exception($"EPUB parsing error: {epubContainerFilePath} file not found in archive.");
			XDocument containerDocument;
			using (var containerStream = containerFileEntry.Open())
			{
				containerDocument = await XmlUtils.LoadDocumentAsync(containerStream).ConfigureAwait(false);
			}

			XNamespace cnsNamespace = "urn:oasis:names:tc:opendocument:xmlns:container";
			var fullPathAttribute = containerDocument.Element(cnsNamespace + "container")?.Element(cnsNamespace + "rootfiles")?.Element(cnsNamespace + "rootfile")
				?.Attribute("full-path");
			if (fullPathAttribute is null) throw new Exception("EPUB parsing error: root file path not found in the EPUB container.");
			return fullPathAttribute.Value;
		}
	}
}