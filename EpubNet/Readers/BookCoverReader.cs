using System;
using System.Linq;
using System.Threading.Tasks;
using EpubNet.RefEntities;

namespace EpubNet.Readers
{
	internal static class BookCoverReader
	{
		public static async Task<byte[]> ReadBookCoverAsync(EpubBookRef bookRef)
		{
			var metaItems = bookRef.Schema.Package.Metadata.MetaItems;
			if (metaItems is null || !metaItems.Any()) return null;

			var coverMetaItem = metaItems.FirstOrDefault(metaItem => string.Compare(metaItem.Name, "cover", StringComparison.OrdinalIgnoreCase) is 0);
			if (coverMetaItem is null) return null;

			if (string.IsNullOrEmpty(coverMetaItem.Content)) throw new Exception("Incorrect EPUB metadata: cover item content is missing.");

			var coverManifestItem = bookRef.Schema.Package.Manifest.FirstOrDefault(manifestItem =>
				string.Compare(manifestItem.Id, coverMetaItem.Content, StringComparison.OrdinalIgnoreCase) == 0);
			if (coverManifestItem is null) throw new Exception($"Incorrect EPUB manifest: item with ID = \"{coverMetaItem.Content}\" is missing.");

			if (!bookRef.Content.Images.TryGetValue(coverManifestItem.Href, out var coverImageContentFileRef))
				throw new Exception($"Incorrect EPUB manifest: item with href = \"{coverManifestItem.Href}\" is missing.");

			var coverImageContent = await coverImageContentFileRef.ReadContentAsBytesAsync().ConfigureAwait(false);
			return coverImageContent;
		}
	}
}