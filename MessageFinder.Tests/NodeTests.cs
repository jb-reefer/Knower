using NUnit.Framework;
using System.Collections.Generic;
using MessageFinder.Interpreter;

namespace MessageFinder.Tests
{
    [TestFixture]
    public class OperatorAndOperandTests
    {
		[Test]
        [Category("Operator")]
        public void TestGreaterThanHardcoded()
        {
            var lhs = new ValueOperand(5.0);
            var rhs = new ValueOperand(-1.5);
            var node = new GreaterThan().SetChildren(lhs, rhs);

            Assert.IsTrue((bool)node.Value().Invoke(null));
        }

        [Test]
        [Category("Operator")]
        public void TestLessThanHardcoded()
        {
            var lhs = new ValueOperand(5.0);
            var rhs = new ValueOperand(-1.5);
            var node = new LessThan().SetChildren(lhs, rhs);

            Assert.IsFalse((bool)node.Value().Invoke(null));
        }

        [Test]
        [Category("Operand")]
        public void BaseCaseFieldOperandTest()
        {
            const int EXPECTED_VALUE = 848;
            var message = new Message
            {
                SizeInBytes = EXPECTED_VALUE
            };

            var operand = new FieldOperand("SizeInBytes");

            Assert.AreEqual(EXPECTED_VALUE, (int)operand.Value().Invoke(message));
        }

        [Test]
        [Category("Operand")]
        public void DotCaseFieldOperandTest()
        {
            var points = new List<Point>
            {
                new Point(),
                new Point(),
                new Point()
            };

            var message = new Message
            {
                Points = points
            };


            var expectedValue = points.Count;

            var operand = new FieldOperand("Points.Count");
            var result = operand.Value().Invoke(message);

            Assert.AreEqual(expectedValue, (int)result);
        }

        [Test]
        [Category("Operator")]
        public void StringEqualsOperatorTest()
        {
            var node = new Equals().SetChildren(new ValueOperand("test"), new ValueOperand("test"));

            Assert.IsTrue((bool)node.Value().Invoke(null));
        }

        [Test]
        [Category("Operator")]
        public void DoubleEqualsOperatorTest()
        {
            var node = new Equals().SetChildren(new ValueOperand(45.10), new ValueOperand(45.10));

            Assert.IsTrue((bool)node.Value().Invoke(null));
        }
    }
}
