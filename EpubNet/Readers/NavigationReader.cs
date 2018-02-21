using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using EpubNet.Schema.Navigation;
using EpubNet.Schema.Opf;
using EpubNet.Utils;

namespace EpubNet.Readers
{
	internal static class NavigationReader
	{
		public static async Task<EpubNavigation> ReadNavigationAsync(ZipArchive epubArchive, string contentDirectoryPath, EpubPackage package)
		{
			var result = new EpubNavigation();
			var tocId = package.Spine.Toc;
			if (String.IsNullOrEmpty(tocId))
			{
				throw new Exception("EPUB parsing error: TOC ID is empty.");
			}

			var tocManifestItem = package.Manifest.FirstOrDefault(item => String.Compare(item.Id, tocId, StringComparison.OrdinalIgnoreCase) == 0);
			if (tocManifestItem == null)
			{
				throw new Exception($"EPUB parsing error: TOC item {tocId} not found in EPUB manifest.");
			}

			var tocFileEntryPath = ZipPathUtils.Combine(contentDirectoryPath, tocManifestItem.Href);
			var tocFileEntry = epubArchive.GetEntry(tocFileEntryPath);
			if (tocFileEntry == null)
			{
				throw new Exception($"EPUB parsing error: TOC file {tocFileEntryPath} not found in archive.");
			}

			if (tocFileEntry.Length > Int32.MaxValue)
			{
				throw new Exception($"EPUB parsing error: TOC file {tocFileEntryPath} is larger than 2 Gb.");
			}

			XDocument containerDocument;
			using (var containerStream = tocFileEntry.Open())
			{
				containerDocument = await XmlUtils.LoadDocumentAsync(containerStream).ConfigureAwait(false);
			}

			XNamespace ncxNamespace = "http://www.daisy.org/z3986/2005/ncx/";
			var ncxNode = containerDocument.Element(ncxNamespace + "ncx");
			if (ncxNode == null)
			{
				throw new Exception("EPUB parsing error: TOC file does not contain ncx element.");
			}

			var headNode = ncxNode.Element(ncxNamespace + "head");
			if (headNode == null)
			{
				throw new Exception("EPUB parsing error: TOC file does not contain head element.");
			}

			var navigationHead = ReadNavigationHead(headNode);
			result.Head = navigationHead;
			var docTitleNode = ncxNode.Element(ncxNamespace + "docTitle");
			if (docTitleNode == null)
			{
				throw new Exception("EPUB parsing error: TOC file does not contain docTitle element.");
			}

			var navigationDocTitle = ReadNavigationDocTitle(docTitleNode);
			result.DocTitle = navigationDocTitle;
			result.DocAuthors = new List<EpubNavigationDocAuthor>();
			foreach (var docAuthorNode in ncxNode.Elements(ncxNamespace + "docAuthor"))
			{
				var navigationDocAuthor = ReadNavigationDocAuthor(docAuthorNode);
				result.DocAuthors.Add(navigationDocAuthor);
			}

			var navMapNode = ncxNode.Element(ncxNamespace + "navMap");
			if (navMapNode == null)
			{
				throw new Exception("EPUB parsing error: TOC file does not contain navMap element.");
			}

			var navMap = ReadNavigationMap(navMapNode);
			result.NavMap = navMap;
			var pageListNode = ncxNode.Element(ncxNamespace + "pageList");
			if (pageListNode != null)
			{
				var pageList = ReadNavigationPageList(pageListNode);
				result.PageList = pageList;
			}

			result.NavLists = new List<EpubNavigationList>();
			foreach (var navigationListNode in ncxNode.Elements(ncxNamespace + "navList"))
			{
				var navigationList = ReadNavigationList(navigationListNode);
				result.NavLists.Add(navigationList);
			}

			return result;
		}

		private static EpubNavigationHead ReadNavigationHead(XElement headNode)
		{
			var result = new EpubNavigationHead();
			foreach (var metaNode in headNode.Elements())
			{
				if (String.Compare(metaNode.Name.LocalName, "meta", StringComparison.OrdinalIgnoreCase) == 0)
				{
					var meta = new EpubNavigationHeadMeta();
					foreach (var metaNodeAttribute in metaNode.Attributes())
					{
						var attributeValue = metaNodeAttribute.Value;
						switch (metaNodeAttribute.Name.LocalName.ToLowerInvariant())
						{
							case "name":
								meta.Name = attributeValue;
								break;
							case "content":
								meta.Content = attributeValue;
								break;
							case "scheme":
								meta.Scheme = attributeValue;
								break;
						}
					}

					if (String.IsNullOrWhiteSpace(meta.Name))
					{
						throw new Exception("Incorrect EPUB navigation meta: meta name is missing.");
					}

					if (meta.Content == null)
					{
						throw new Exception("Incorrect EPUB navigation meta: meta content is missing.");
					}

					result.Add(meta);
				}
			}

			return result;
		}

		private static EpubNavigationDocTitle ReadNavigationDocTitle(XElement docTitleNode)
		{
			var result = new EpubNavigationDocTitle();
			foreach (var textNode in docTitleNode.Elements())
			{
				if (String.Compare(textNode.Name.LocalName, "text", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result.Add(textNode.Value);
				}
			}

			return result;
		}

		private static EpubNavigationDocAuthor ReadNavigationDocAuthor(XElement docAuthorNode)
		{
			var result = new EpubNavigationDocAuthor();
			foreach (var textNode in docAuthorNode.Elements())
			{
				if (String.Compare(textNode.Name.LocalName, "text", StringComparison.OrdinalIgnoreCase) == 0)
				{
					result.Add(textNode.Value);
				}
			}

			return result;
		}

		private static EpubNavigationMap ReadNavigationMap(XElement navigationMapNode)
		{
			var result = new EpubNavigationMap();
			foreach (var navigationPointNode in navigationMapNode.Elements())
			{
				if (String.Compare(navigationPointNode.Name.LocalName, "navPoint", StringComparison.OrdinalIgnoreCase) == 0)
				{
					var navigationPoint = ReadNavigationPoint(navigationPointNode);
					result.Add(navigationPoint);
				}
			}

			return result;
		}

		private static EpubNavigationPoint ReadNavigationPoint(XElement navigationPointNode)
		{
			var result = new EpubNavigationPoint();
			foreach (var navigationPointNodeAttribute in navigationPointNode.Attributes())
			{
				var attributeValue = navigationPointNodeAttribute.Value;
				switch (navigationPointNodeAttribute.Name.LocalName.ToLowerInvariant())
				{
					case "id":
						result.Id = attributeValue;
						break;
					case "class":
						result.Class = attributeValue;
						break;
					case "playOrder":
						result.PlayOrder = attributeValue;
						break;
				}
			}

			if (String.IsNullOrWhiteSpace(result.Id))
			{
				throw new Exception("Incorrect EPUB navigation point: point ID is missing.");
			}

			result.NavigationLabels = new List<EpubNavigationLabel>();
			result.ChildNavigationPoints = new List<EpubNavigationPoint>();
			foreach (var navigationPointChildNode in navigationPointNode.Elements())
			{
				switch (navigationPointChildNode.Name.LocalName.ToLowerInvariant())
				{
					case "navlabel":
						var navigationLabel = ReadNavigationLabel(navigationPointChildNode);
						result.NavigationLabels.Add(navigationLabel);
						break;
					case "content":
						var content = ReadNavigationContent(navigationPointChildNode);
						result.Content = content;
						break;
					case "navpoint":
						var childNavigationPoint = ReadNavigationPoint(navigationPointChildNode);
						result.ChildNavigationPoints.Add(childNavigationPoint);
						break;
				}
			}

			if (!result.NavigationLabels.Any())
			{
				throw new Exception($"EPUB parsing error: navigation point {result.Id} should contain at least one navigation label.");
			}

			if (result.Content is null)
			{
				throw new Exception($"EPUB parsing error: navigation point {result.Id} should contain content.");
			}

			return result;
		}

		private static EpubNavigationLabel ReadNavigationLabel(XElement navigationLabelNode)
		{
			var result = new EpubNavigationLabel();
			var navigationLabelTextNode = navigationLabelNode.Element(navigationLabelNode.Name.Namespace + "text");
			if (navigationLabelTextNode is null)
			{
				throw new Exception("Incorrect EPUB navigation label: label text element is missing.");
			}

			result.Text = navigationLabelTextNode.Value;
			return result;
		}

		private static EpubNavigationContent ReadNavigationContent(XElement navigationContentNode)
		{
			var result = new EpubNavigationContent();
			foreach (var navigationContentNodeAttribute in navigationContentNode.Attributes())
			{
				var attributeValue = navigationContentNodeAttribute.Value;
				switch (navigationContentNodeAttribute.Name.LocalName.ToLowerInvariant())
				{
					case "id":
						result.Id = attributeValue;
						break;
					case "src":
						result.Source = attributeValue;
						break;
				}
			}

			if (String.IsNullOrWhiteSpace(result.Source))
			{
				throw new Exception("Incorrect EPUB navigation content: content source is missing.");
			}

			return result;
		}

		private static EpubNavigationPageList ReadNavigationPageList(XElement navigationPageListNode)
		{
			var result = new EpubNavigationPageList();
			foreach (var pageTargetNode in navigationPageListNode.Elements())
			{
				if (String.Compare(pageTargetNode.Name.LocalName, "pageTarget", StringComparison.OrdinalIgnoreCase) == 0)
				{
					var pageTarget = ReadNavigationPageTarget(pageTargetNode);
					result.Add(pageTarget);
				}
			}

			return result;
		}

		private static EpubNavigationPageTarget ReadNavigationPageTarget(XElement navigationPageTargetNode)
		{
			var result = new EpubNavigationPageTarget();
			foreach (var navigationPageTargetNodeAttribute in navigationPageTargetNode.Attributes())
			{
				var attributeValue = navigationPageTargetNodeAttribute.Value;
				switch (navigationPageTargetNodeAttribute.Name.LocalName.ToLowerInvariant())
				{
					case "id":
						result.Id = attributeValue;
						break;
					case "value":
						result.Value = attributeValue;
						break;
					case "type":
						if (!Enum.TryParse(attributeValue, out EpubNavigationPageTargetType type))
						{
							throw new Exception($"Incorrect EPUB navigation page target: {attributeValue} is incorrect value for page target type.");
						}

						result.Type = type;
						break;
					case "class":
						result.Class = attributeValue;
						break;
					case "playOrder":
						result.PlayOrder = attributeValue;
						break;
				}
			}

			if (result.Type is default)
			{
				throw new Exception("Incorrect EPUB navigation page target: page target type is missing.");
			}

			foreach (var navigationPageTargetChildNode in navigationPageTargetNode.Elements())
				switch (navigationPageTargetChildNode.Name.LocalName.ToLowerInvariant())
				{
					case "navlabel":
						var navigationLabel = ReadNavigationLabel(navigationPageTargetChildNode);
						result.NavigationLabels.Add(navigationLabel);
						break;
					case "content":
						var content = ReadNavigationContent(navigationPageTargetChildNode);
						result.Content = content;
						break;
				}
			if (!result.NavigationLabels.Any())
			{
				throw new Exception("Incorrect EPUB navigation page target: at least one navLabel element is required.");
			}

			return result;
		}

		private static EpubNavigationList ReadNavigationList(XElement navigationListNode)
		{
			var result = new EpubNavigationList();
			foreach (var navigationListNodeAttribute in navigationListNode.Attributes())
			{
				var attributeValue = navigationListNodeAttribute.Value;
				switch (navigationListNodeAttribute.Name.LocalName.ToLowerInvariant())
				{
					case "id":
						result.Id = attributeValue;
						break;
					case "class":
						result.Class = attributeValue;
						break;
				}
			}

			foreach (var navigationListChildNode in navigationListNode.Elements())
			{
				switch (navigationListChildNode.Name.LocalName.ToLowerInvariant())
				{
					case "navlabel":
						var navigationLabel = ReadNavigationLabel(navigationListChildNode);
						result.NavigationLabels.Add(navigationLabel);
						break;
					case "navTarget":
						var navigationTarget = ReadNavigationTarget(navigationListChildNode);
						result.NavigationTargets.Add(navigationTarget);
						break;
				}
			}

			if (!result.NavigationLabels.Any())
			{
				throw new Exception("Incorrect EPUB navigation page target: at least one navLabel element is required.");
			}

			return result;
		}

		private static EpubNavigationTarget ReadNavigationTarget(XElement navigationTargetNode)
		{
			var result = new EpubNavigationTarget();
			foreach (var navigationPageTargetNodeAttribute in navigationTargetNode.Attributes())
			{
				var attributeValue = navigationPageTargetNodeAttribute.Value;
				switch (navigationPageTargetNodeAttribute.Name.LocalName.ToLowerInvariant())
				{
					case "id":
						result.Id = attributeValue;
						break;
					case "value":
						result.Value = attributeValue;
						break;
					case "class":
						result.Class = attributeValue;
						break;
					case "playOrder":
						result.PlayOrder = attributeValue;
						break;
				}
			}

			if (String.IsNullOrWhiteSpace(result.Id))
			{
				throw new Exception("Incorrect EPUB navigation target: navigation target ID is missing.");
			}

			foreach (var navigationTargetChildNode in navigationTargetNode.Elements())
			{
				switch (navigationTargetChildNode.Name.LocalName.ToLowerInvariant())
				{
					case "navlabel":
						var navigationLabel = ReadNavigationLabel(navigationTargetChildNode);
						result.NavigationLabels.Add(navigationLabel);
						break;
					case "content":
						var content = ReadNavigationContent(navigationTargetChildNode);
						result.Content = content;
						break;
				}
			}

			if (!result.NavigationLabels.Any())
			{
				throw new Exception("Incorrect EPUB navigation target: at least one navLabel element is required.");
			}

			return result;
		}
	}
}