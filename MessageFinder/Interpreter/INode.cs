using System;

namespace MessageFinder.Interpreter
{
    public interface INode
    {
        Func<object, object> Value();
        INode SetChildren(INode left, INode right);
    }
}