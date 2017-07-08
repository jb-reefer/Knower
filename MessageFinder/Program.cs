using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using log4net;
using MessageFinder.Interpreter;
using NDesk.Options;
using Newtonsoft.Json.Linq;

namespace MessageFinder
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(string[] args)
        {
            var help = false;
            var filename = string.Empty;
            var query = string.Empty;
            var outputArchive = string.Empty;


            var options = new OptionSet
            {
                {"f|file=", "File to read json messages from. String.", value => filename = value},
                {"o|output=", "Archive to write output. Default is ./{query name}.", value => outputArchive = value},
                {"q|query=", "Filter query, wrapped in quotes. String.", value => query = value},
                {"h|?|help", "Print this help message.", value => help = value != null}
            };

            options.Parse(args);

            if (help || string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(query))
            {
                var writer = new StringWriter();
                options.WriteOptionDescriptions(writer);

                Log.Info(writer.ToString());
                Environment.Exit(0);
            }

            try
            {
                Execute(query, outputArchive, filename);
            }
            catch (Exception e)
            {
                Log.Error("Encountered unexpected error during execution:", e);
                Environment.Exit(0);
            }

        }

        private static void Execute(string query, string outputArchive, string filename)
        { 
            Log.Info("Reading messages...");

            var messageReader = new MessageReader();
            var messages = messageReader.ReadFile(filename);

            Log.Info("Reading messages complete.");
            Log.Info($"Read {messages.Count} messages.");

            Log.Info("Building AST...");

            var ast = Query.Compile(query);

			Log.Info("AST built succesfully.");

            var matches = ast.Value();

            Log.Info("Processing messages.");

            var matchedMessages = messages.Where(message => (bool) matches.Invoke(message)).ToList();

            Log.Info("Processed messages.");
            Log.Info("Saving result set.");

            if (outputArchive == string.Empty)
            {
				outputArchive = query.Replace(" ", string.Empty) + ".zip";
            }

            var outputFileName = @"temp/messages.json";

            Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));
            File.WriteAllText(outputFileName, JArray.FromObject(matchedMessages).ToString());

            if (File.Exists(outputArchive))
            {
                File.Delete(outputArchive);
            }

			ZipFile.CreateFromDirectory("temp", outputArchive);

            Log.Info($"{matchedMessages.Count} matching messages saved in archive @ {outputArchive}");
            Log.Info("Processing complete, exiting.");
        }

    }
}
