using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace EpubNet.Utils
{
	internal static class XmlUtils
	{
		public static async Task<XDocument> LoadDocumentAsync(Stream stream)
		{
			using (var memoryStream = new MemoryStream())
			{
				await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
				memoryStream.Position = 0;
				var xmlReaderSettings = new XmlReaderSettings
				{
					DtdProcessing = DtdProcessing.Ignore,
					Async = true
				};
				using (XmlReader.Create(memoryStream, xmlReaderSettings))
				{
					return XDocument.Load(memoryStream);
				}
			}
		}
	}
}