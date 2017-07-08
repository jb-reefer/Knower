using MessageFinder.Interpreter;
using NUnit.Framework;

namespace MessageFinder.Tests
{
    [TestFixture]
    public class ParenthesisTests
    {
        [Test]
        [TestCase("(true and false)", false)]
        [TestCase("(true and true) and (false or true)", true)]
        [TestCase("(true and true) and false", false)]
		[TestCase("(5 greater than 4) and false", false)]
		[TestCase("(5 greater than 4) and true", true)]
		[TestCase("(5 greater than 4)", true)]
        [TestCase("(5 greater than 4) or (4 less than 2)", true)]
		public void TestParens(string query, bool truthy)
        {
            var parsedOperator = Query.Compile(query);

            Assert.AreEqual(truthy, parsedOperator.Value().Invoke(null));
        }

        [Test]
        [TestCase("(true and false)")]
        [TestCase("(true and true) and (false or true)")]
        [TestCase("((true and true) and (false or true))")]
        [TestCase("(true and true) and false")]
        public void ParensSplitTest(string query)
        {
            var result = Query.Cut(query);

            Assert.AreEqual(3, result.Count);
        }
    }
}
