using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using EpubNet.Entities;
using EpubNet.Utils;

namespace EpubNet.RefEntities
{
    public abstract class EpubContentFileRef
    {
        private readonly EpubBookRef _epubBookRef;

        protected EpubContentFileRef(EpubBookRef epubBookRef)
        {
            _epubBookRef = epubBookRef;
        }

        public string FileName { get; set; }
        public EpubContentType ContentType { get; set; }
        public string ContentMimeType { get; set; }


        public async Task<byte[]> ReadContentAsBytesAsync()
        {
            var contentFileEntry = GetContentFileEntry();
            var content = new byte[(int) contentFileEntry.Length];
            using (var contentStream = OpenContentStream(contentFileEntry))
            using (var memoryStream = new MemoryStream(content))
            {
                await contentStream.CopyToAsync(memoryStream).ConfigureAwait(false);
            }

            return content;
        }

        public async Task<string> ReadContentAsTextAsync()
        {
            using (var contentStream = GetContentStream())
            using (var streamReader = new StreamReader(contentStream))
            {
                return await streamReader.ReadToEndAsync().ConfigureAwait(false);
            }
        }

        private Stream GetContentStream() => OpenContentStream(GetContentFileEntry());

        private ZipArchiveEntry GetContentFileEntry()
        {
            var contentFilePath = ZipPathUtils.Combine(_epubBookRef.Schema.ContentDirectoryPath, FileName);
            var contentFileEntry = _epubBookRef.EpubArchive.GetEntry(contentFilePath);
            if (contentFileEntry is null) throw new Exception($"EPUB parsing error: file {contentFilePath} not found in archive.");

            if (contentFileEntry.Length > int.MaxValue) throw new Exception($"EPUB parsing error: file {contentFilePath} is bigger than 2 Gb.");

            return contentFileEntry;
        }

        private Stream OpenContentStream(ZipArchiveEntry contentFileEntry)
        {
            var contentStream = contentFileEntry.Open();
            if (contentStream is null) throw new Exception($"Incorrect EPUB file: content file \"{FileName}\" specified in manifest is not found.");

            return contentStream;
        }
    }
}