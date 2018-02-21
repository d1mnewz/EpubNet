namespace EpubNet.Utils
{
	internal static class ZipPathUtils
	{
		public static string GetDirectoryPath(string filePath)
		{
			var lastSlashIndex = filePath.LastIndexOf('/');
			return lastSlashIndex is -1 ? string.Empty : filePath.Substring(0, lastSlashIndex);
		}

		public static string Combine(string directory, string fileName) => string.IsNullOrEmpty(directory) ? fileName : string.Concat(directory, "/", fileName);
	}
}