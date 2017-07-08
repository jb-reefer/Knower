using System;
using System.Collections.Generic;
using System.Linq;
using MessageFinder.Interpreter;
using NUnit.Framework;

namespace MessageFinder.Tests
{
    [TestFixture]
    [Description("The query parser can work on ANY object, not just messages. Here's some examples.")]
    public class DifferentUseCasesTests
    {
        private class ExampleClass
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }

        [Test]
        [TestCase("IntValue equals 6", 1)]
        [TestCase("StringValue equals Example", 2)]
        [TestCase("StringValue equals Example and (IntValue equals 6)", 1 )]
        public void SimpleClassTest(string query, int size)
        {
            var inputSet = new List<ExampleClass>
            {
                new ExampleClass
                {
                    StringValue = "Example",
                    IntValue = 6
                },
                new ExampleClass { StringValue = "Example", IntValue = 5}
            };
            
            var parser = Query.Compile(query);

            var resultSet = inputSet.Where( member => (bool) parser.Value().Invoke(member)).ToList();

            Assert.AreEqual(size, resultSet.Count());
        }

        [Test]
        [TestCase("Month equals 5", true)]
        [TestCase("Month equals 12", false)]
        [TestCase("Year equals 2017", true)]
        [TestCase("Year greater than 1950", true)]
        public void DateTimeTest(string query, bool truthy)
        {
            var today = DateTime.Today;

            var parser = Query.Compile(query);

            Assert.AreEqual(truthy, parser.Value().Invoke(today));
        }

        [Test]
        [Description("It can also work on heterogenous datasets")]
        [TestCase("Length equals 5", true)]
        [TestCase("Length greater than 4", true)]
        [TestCase("Length lessthan 1", false)]
        public void MixedBag(string input, bool truthy)
        {
            var inputSet = new List<object>
             {
                { new string[5] },
                { "fiveL" }
             };

            var query = Query.Compile(input).Value();

            var resultSet = inputSet.Where(member => (bool) query.Invoke(member)).ToList();

            Assert.AreEqual(truthy, resultSet.Count == 2);
        }

    }
}