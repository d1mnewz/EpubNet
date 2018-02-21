using System.Diagnostics.CodeAnalysis;

namespace EpubNet.Schema.Opf
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class EpubSpineItemRef
	{
		public string IdRef { get; set; }
		public bool IsLinear { get; set; }

		public override string ToString() => $"IdRef: {IdRef}";
	}
}