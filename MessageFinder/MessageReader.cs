using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MessageFinder
{
    public class MessageReader
    {
        // I did this with a json stream reader at first, but using the preprocessor required some ugly code
        public List<Message> ReadFile(string filename)
        {
            var builder = new StringBuilder(File.ReadAllText(filename));
            builder = PreProcess(builder);

            var cache = JArray.Parse(builder.ToString());
            return cache.ToObject<List<Message>>();
        }

        private StringBuilder PreProcess(StringBuilder builder)
        {
            // Handle jsonlines (I know, but I only had a week and this was last)
            // I am sure there is a better way to do this.
            builder.Insert(0, '[');
            builder.Append(']');
            return builder.Replace("}{", "}, {");
        }
    }
}