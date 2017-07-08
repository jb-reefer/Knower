using System;

namespace MessageFinder.Interpreter
{
    public abstract class Operator : INode
    {
        protected INode Left { get; set; }
        protected INode Right { get; set; }

        public INode SetChildren(INode left, INode right)
        {
            Left = left;
            Right = right;

            return this;
        }

        public abstract Func<object, object> Value();
    }

    // OPERATORS: greater than, less than, equals, AND, OR
    public class GreaterThan : Operator
    {
        public override Func<object, object> Value()
        {
            return message => Convert.ToDouble(Left.Value().Invoke(message)) > Convert.ToDouble(Right.Value().Invoke(message));
        }
    }

    public class LessThan : Operator
    {
        public override Func<object, object> Value()
        {
            return message => Convert.ToDouble(Left.Value().Invoke(message)) < Convert.ToDouble(Right.Value().Invoke(message));
        }
    }

    public class Equals : Operator
    {
        public override Func<object, object> Value()
        {
            // This addresses a specific case where Equals can act on any object, but sometimes compares "6.0" and 6 - which are not the same
            // but are in our duck-typed world. 
            return message => Left.Value().Invoke(message).ToString().Equals(Right.Value().Invoke(message).ToString());
        }
    }

    public class And : Operator
    {
        public override Func<object, object> Value()
        {
            // To support "xy equals True"
            return message => Convert.ToBoolean(Left.Value().Invoke(message)) && Convert.ToBoolean(Right.Value().Invoke(message));
        }
    }

    public class Or : Operator
    {
        public override Func<object, object> Value()
        {
            return message => Convert.ToBoolean(Left.Value().Invoke(message)) || Convert.ToBoolean(Right.Value().Invoke(message));
        }
    }
}