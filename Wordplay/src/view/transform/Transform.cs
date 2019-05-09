using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommandLine;
using Wordplay.Model.Transform;

namespace Wordplay.View.Transform
{
	static class Transform
	{
		private static WordTransformer Transformer;
		private static string WelcomeMessage =
@"Transform one word into another by changing one character at a time.
Valid queries have the form:
	<startWord> <endWord> [-s]
If the -s flag is given, only substitutions will be allowed when transforming words.";

		public static void Run(string dictionaryFilename)
		{
			Initialize(dictionaryFilename);
			Console.WriteLine(WelcomeMessage);

			while (true)
			{
				Console.Write("\nEnter a query ('q' to exit): ");
				string query = Console.ReadLine().Trim().ToLower();

				if (query == "q")
					break;

				string[] queryArgs = query.Split(' ');
				Parser.Default.ParseArguments<TransformOptions>(queryArgs).WithParsed(options =>
				{
					RunSearch(options);
				});
			}
		}

		private static void Initialize(string dictionaryFilename)
		{
			Console.WriteLine("Parsing dictionary...");
			var allWords = GetAllWords(dictionaryFilename);

			Console.WriteLine("Building word graph (this may take a few moments)...");
			var wordGraph = new WordGraph(allWords);

			Transformer = new WordTransformer(wordGraph);
		}

		private static List<string> GetAllWords(string dictionaryFilename)
		{
			var words = new List<string>();
			using (var dictionaryFile = new StreamReader(dictionaryFilename))
			{
				while (!dictionaryFile.EndOfStream)
				{
					string word = dictionaryFile.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(word))
						words.Add(word);
				}
			}

			return words;
		}

		private static void RunSearch(TransformOptions options)
		{
			if (options.SubstitutionsOnly && options.Start.Length != options.End.Length)
			{
				Console.WriteLine(
					"When only substitutions are allowed, the start and end words must be the same length."
				);
				return;
			}

			var path = Transformer.Transform(options.Start, options.End, options.SubstitutionsOnly);
			if (path.Count() == 0)
			{
				Console.WriteLine("Looks like this can't be done :(");
			}
			else
			{
				Console.WriteLine("Done! Here's how:");
				foreach (string word in path)
				{
					Console.WriteLine(word);
				}
			}
		}
	}
}