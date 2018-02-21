using System;

namespace EpubNet.Utils
{
	internal static class ZipPathUtils
	{
		public static string GetDirectoryPath(string filePath)
		{
			var lastSlashIndex = filePath.LastIndexOf('/');
			return lastSlashIndex is -1 ? String.Empty : filePath.Substring(0, lastSlashIndex);
		}

		public static string Combine(string directory, string fileName) => String.IsNullOrEmpty(directory) ? fileName : String.Concat(directory, "/", fileName);
	}
}