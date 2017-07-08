using MessageFinder.Interpreter;
using NUnit.Framework;

namespace MessageFinder.Tests
{
    [TestFixture]
    public class ExpressionTests
    {
        [Test]
        [TestCase("5 greater than 1", true)]
        [TestCase("5 less than 1", false)]
        [TestCase("-6 less than 5", true)]
        [TestCase("5 equals 1", false)]
        [TestCase("6 equals 6", true)]
        [TestCase("6 equals     6", true)]
        [TestCase("-6 equals -6", true)]
        [TestCase("(-6 equals -6) and (6 equals 6)", true)]
        [TestCase("(-6 equals -6) and (6 equals -6)", false)]
        [TestCase("(-6 greater than -6) and (6 equals -6)", false)]
        [TestCase("(-6 less than -6) and (6 equals -6)", false)]
        public void IntExpressionTest(string query, bool truthy)
        {
            var parser = Query.Compile(query);

            Assert.AreEqual(truthy, parser.Value().Invoke(null));
        }

        [Test]
        [TestCase("true and false", false)]
        [TestCase("true or false", true)]
        [TestCase("true      or false", true)]
        [TestCase("true equals false", false)]
        [TestCase("true equals true", true)]
        [TestCase("false equals false", true)]
        [TestCase("true and true", true)]
        [TestCase("true or true", true)]
        [TestCase("true and false", false)]
        [TestCase("false and false", false)]
        [TestCase("false or false", false)]
        [TestCase("5.7 > 5.0 AND 1 < 100 AND 1.0 < 10 AND 5 > 4", true)]
        [TestCase("5.7 > 5.0 AND 1 < 100 AND 1.0 < 10 AND 5 > 6", false)]
        public void BoolExpressionTest(string query, bool truthy)
        {
            var parser = Query.Compile(query);

            Assert.AreEqual(truthy, parser.Value().Invoke(null));
        }
    }
}