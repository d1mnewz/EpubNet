using FluentAssertions;
using Xunit;

namespace EpubNet.Tests
{
	public class Tests
	{
		[Fact]
		public void Initial()
		{
			true.Should().BeTrue();
		}
	}
}