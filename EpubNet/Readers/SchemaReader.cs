using System.IO.Compression;
using System.Threading.Tasks;
using EpubNet.Entities;
using EpubNet.Utils;

namespace EpubNet.Readers
{
    internal static class SchemaReader
    {
        public static async Task<EpubSchema> ReadSchemaAsync(ZipArchive epubArchive)
        {
            var result = new EpubSchema();
            var rootFilePath = await RootFilePathReader.GetRootFilePathAsync(epubArchive).ConfigureAwait(false);
            var contentDirectoryPath = ZipPathUtils.GetDirectoryPath(rootFilePath);
            result.ContentDirectoryPath = contentDirectoryPath;
            var package = await PackageReader.ReadPackageAsync(epubArchive, rootFilePath).ConfigureAwait(false);
            result.Package = package;
            var navigation = await NavigationReader.ReadNavigationAsync(epubArchive, contentDirectoryPath, package).ConfigureAwait(false);
            result.Navigation = navigation;
            return result;
        }
    }
}
