using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageFinder.Interpreter
{
    public static class Query
    {
        //  This is a naive implementation, and can be optimized
        //  Not now though, premature optimization is the root of all evil
        public static List<string> Cut(string input)
        {
            var wrappedInput = WrapWithBrackets(input);

            if (wrappedInput.Contains("("))
            {
                var output = new List<string>();
                var endPoints = OuterParenMatcher(wrappedInput);

                // Case: (5 equals 2) ONLY
                if (endPoints.Item2 == wrappedInput.Length - 1)
                {
                    wrappedInput = wrappedInput.Substring(endPoints.Item1, endPoints.Item2 - endPoints.Item1);
                    return Cut(wrappedInput);
                }

                // Case: (5 equals 2) and true
                var firstFlag = wrappedInput[0] == '(';

                var parenChunk = wrappedInput.Substring(endPoints.Item1, endPoints.Item2 - endPoints.Item1);

                wrappedInput = wrappedInput.Remove(endPoints.Item1 - 1, endPoints.Item2 - (endPoints.Item1 - 1) + 1);
                var slices = wrappedInput.Split(new[] {' '}, 2, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (firstFlag)
                {
                    output.Add(parenChunk);
                    output.AddRange(slices);
                }
                else
                {
                    output.AddRange(slices);
                    output.Add(parenChunk);
                }

                return output;
            }

            return wrappedInput.Split(new[] {' '}, 3, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private static string WrapWithBrackets(string input)
        {
            var chunks = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            // Case: Short, parsable query like 5 greaterthan 4
            if (chunks.Length <= 3) return input;

            // Case: Long, unbraced queries like "x > y AND y < z AND q < u AND p > z"

            // Every 3 words, append a closing brace
            // This works because all expressions in this DSL are tripartite and 3 words long 
            // If it's stupid and it works, it's not stupid.
            var buffer = new StringBuilder("(");
            var closedFlag = false;
            var accumulator = 0;
            for (var i = 0; i < chunks.Length; i++, accumulator++)
            {
                if (accumulator == 3)
                {
                    buffer.Append(")");
                    closedFlag = true;
                }

                buffer.Append(' ');

                if (accumulator == 4)
                {
                    // Don't open a new brace if we're not going to close it in time
                    if (i + 3 < chunks.Length)
                    {
                        buffer.Append("(");
                        closedFlag = false;
                    }
                    accumulator = 0;
                }


                buffer.Append(chunks[i]);
            }

            if (!closedFlag) buffer.Append(")");


            return buffer.ToString().Trim();
        }

        private static Tuple<int, int> OuterParenMatcher(string input)
        {
            var opening = input.IndexOf('(');

            var stackCounter = 0;

            var closing = opening + 1;
            for (; closing < input.Length; closing++)
            {
                if (input[closing] == '(')
                {
                    stackCounter++;
                }
                else if (input[closing] == ')')
                {
                    // Found the closing brace
                    if (stackCounter == 0) break;

                    stackCounter--;
                }
            }

            return new Tuple<int, int>(opening + 1, closing);
        }

        public static INode Compile(string input)
        {
            input = QueryPreprocessor.Process(input);

            if (input.Contains(' '))
            {
                var parts = Cut(input);

                // Infix, left hand side, right hand side
                return Compile(parts[1]).SetChildren(Compile(parts[0]), Compile(parts[2]));
            }

            switch (input.ToLower())
            {
                case ">":
                case "greaterthan":
                    return new GreaterThan();
                case "<":
                case "lessthan":
                    return new LessThan();
                case "==":
                case "equals":
                    return new Equals();
                case "&&":
                case "and":
                    return new And();
                case "or":
                case "||":
                    return new Or();
            }

            return new DynamicOperand(input);
        }
    }
}