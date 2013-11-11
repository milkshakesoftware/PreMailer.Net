using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PreMailer.Net.Tests
{
	[TestClass]
	public class CssSpecificityTests
	{
		[TestMethod]
		public void PlusOperator_OneIdForBothInstances_ReturnsTwoIds()
		{
			var first = new CssSpecificity(1, 0, 0);
			var second = new CssSpecificity(1, 0, 0);
			var result = first + second;
			Assert.AreEqual(2, result.Ids);
		}
		
		[TestMethod]
		public void PlusOperator_OneClassForBothInstances_ReturnsTwoClasses()
		{
			var first = new CssSpecificity(0, 1, 0);
			var second = new CssSpecificity(0, 1, 0);
			var result = first + second;
			Assert.AreEqual(2, result.Classes);
		}
		
		[TestMethod]
		public void PlusOperator_OneElementForBothInstances_ReturnsTwoElements()
		{
			var first = new CssSpecificity(0, 0, 1);
			var second = new CssSpecificity(0, 0, 1);
			var result = first + second;
			Assert.AreEqual(2, result.Elements);
		}
	}
}