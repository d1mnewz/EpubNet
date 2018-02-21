﻿using System.Threading.Tasks;

namespace EpubNet.RefEntities
{
	public class EpubByteContentFileRef : EpubContentFileRef
	{
		public EpubByteContentFileRef(EpubBookRef epubBookRef)
			: base(epubBookRef)
		{
		}


		public Task<byte[]> ReadContentAsync()
		{
			return ReadContentAsBytesAsync();
		}
	}
}