using Xunit;

namespace PreMailer.Net.Tests
{
	public class CssSpecificityTests
	{
		[Fact]
		public void PlusOperator_OneIdForBothInstances_ReturnsTwoIds()
		{
			var first = new CssSpecificity(1, 0, 0);
			var second = new CssSpecificity(1, 0, 0);
			var result = first + second;
			Assert.Equal(2, result.Ids);
		}
		
		[Fact]
		public void PlusOperator_OneClassForBothInstances_ReturnsTwoClasses()
		{
			var first = new CssSpecificity(0, 1, 0);
			var second = new CssSpecificity(0, 1, 0);
			var result = first + second;
			Assert.Equal(2, result.Classes);
		}
		
		[Fact]
		public void PlusOperator_OneElementForBothInstances_ReturnsTwoElements()
		{
			var first = new CssSpecificity(0, 0, 1);
			var second = new CssSpecificity(0, 0, 1);
			var result = first + second;
			Assert.Equal(2, result.Elements);
		}
	}
}