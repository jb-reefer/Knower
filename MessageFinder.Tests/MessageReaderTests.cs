using System;
using System.Configuration;
using System.Linq;
using MessageFinder.Interpreter;
using NUnit.Framework;


namespace MessageFinder.Tests
{
    [TestFixture]
    public class MessageMatchTests
    {
        private readonly string fileName = ConfigurationManager.AppSettings["TestFileLocation"];

        [TestCase("SizeInBytes greater than 400")]
        [TestCase("DeviceId equals jakesdevice")]
        [TestCase("Points.Count greater than 0")]
        [TestCase("(SizeInBytes less than 600 AND Points.Count greater than 1) OR ConnectorName equals DeviceSimulator")]
        public void TestExamplesFromSpecCompile(string query)
        {
            var reader = new MessageReader();
            var messages = reader.ReadFile(fileName);

            var ast = Query.Compile(query);

            var tester = new TestDelegate(
                                          () => messages.Where(message => (bool)ast.Value().Invoke(message))
                                         );

            Assert.DoesNotThrow(tester);
        }

        [TestCase("SizeInBytes greater than 400")]
        [TestCase("SizeInBytes less than 410")]
        [TestCase("DeviceId equals jakesdevice")]
        [TestCase("Points.Count greater than 0")]
        [TestCase("AssetKey equals 02073a43-10fd-41b5-ad73-a75860ce2808")]
        [TestCase("UserKey equals 9232c8bf-d256-432f-b7ac-c22831943750")]
        [TestCase("Points.Count greater than 0 and AssetKey equals 02073a43-10fd-41b5-ad73-a75860ce2808")]
        [TestCase("Points.Count greater than 0 and UserKey equals 9232c8bf-d256-432f-b7ac-c22831943750")]
        [TestCase("(SizeInBytes less than 600 AND Points.Count greater than 1) OR ConnectorName equals DeviceSimulator")]
        [TestCase("(SizeInBytes greater than 600 AND Points.Count greater than 1) OR ConnectorName equals DeviceSimulator")]
        [TestCase("(SizeInBytes less than 600 AND (Points.Count greater than 1)) OR ConnectorName equals DeviceSimulator")]
        [TestCase("(SizeInBytes less than 600 AND (Points.Count greater than 1) OR 600 > 500 AND Points.Count greater than 1) OR ConnectorName equals DeviceSimulator")]
        public void TestOutputFromSpecCompile(string query)
        {
            var reader = new MessageReader();

            var messages = reader.ReadFile(fileName);

            var parser = Query.Compile(query);
            var results = messages.Where(message=> (bool)parser.Value().Invoke(message)).ToList();

            Console.WriteLine($"Results: {results.Count}");
            results.ForEach(Console.WriteLine);

            Assert.IsNotNull(results);
            Assert.IsNotEmpty(results);
        }
    }

    [TestFixture]
    class MessageReaderTests
    {
        private readonly string fileName = ConfigurationManager.AppSettings["TestFileLocation"];

        [Test]
        public void ReadExampleFile()
        {
            var messageReader = new MessageReader();

            Assert.DoesNotThrow(() => messageReader.ReadFile(fileName));
        }

        [Test]
        public void VerifyDataStructure()
        {
            var messageReader = new MessageReader();

            var messages = messageReader.ReadFile(fileName);

            var devices = from e in messages
                group e by new {id = e.DeviceId}
                into g
                select new {g.Key.id, count = g.Count()};

            Assert.IsNotNull(devices);
            Assert.IsNotEmpty(devices);
        }
    }
}