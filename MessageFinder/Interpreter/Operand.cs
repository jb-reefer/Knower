using System;
using System.Linq;

namespace MessageFinder.Interpreter
{
    public abstract class Operand : INode
    {
        public abstract Func<object, object> Value();

        public INode SetChildren(INode left, INode right)
        {
            throw new InvalidOperationException("Cannot set children of operand.");
        }
    }

    public class DynamicOperand : Operand
    {
        private  string _value;

        public DynamicOperand(string value)
        {
            _value = value;
        }

        private static bool IsProperty(object target, string input)
        {
            if (input.Contains('.'))
            {
                input = input.Split('.').FirstOrDefault();
            }

            // Saving this after first population gives you a speed boost, but restricts you to a single target type
            // This way, you can actually use any set of objects (that all have the same properties) you want to filter!
            var fieldNames = target.GetType().GetProperties().Select(property => property.Name);
            return fieldNames.Contains(input);
        }

        public override Func<object, object> Value()
        {
            Func<object, object> output = (targetObject) =>
            {
                // Target Object is null for value operands
                if(targetObject != null && IsProperty(targetObject, _value))
                {
                    return new FieldOperand(_value).Value().Invoke(targetObject);
                }

                return new ValueOperand(_value).Value().Invoke(targetObject);
            };

            return output;
        }
    }

    public class ValueOperand : Operand
    {
        private readonly object _value;

        public ValueOperand(object value)
        {
            _value = value;
        }

        public override Func<object, object> Value()
        {
            return message => _value;
        }
    }

    public class FieldOperand : Operand
    {
        private readonly string _refinementDefinition;

        public FieldOperand(string refinementDefinition)
        {
            _refinementDefinition = refinementDefinition;
        }

        public override Func<object, object> Value()
        {
            return message => GetPropertyValue(message, _refinementDefinition);
        }

        private object GetPropertyValue(object target, string refinement)
        {
            var parts = refinement.Split(new[] { '.' }, 2);

            // Base case such "SizeInBytes" 
            var value = target.GetType().GetProperty(parts.First()).GetValue(target);

            // Recursive case such "Points.Count"
            // N.B.: Would convert entire query to and use uppercase for everything, but would break Points.Count requirement
            if (refinement.Contains('.')) return GetPropertyValue(value, parts[1]);

            return value;
        }
    }
}
