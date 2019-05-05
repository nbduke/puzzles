using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommandLine;
using Tools.Algorithms.Search;
using Wordplay.Model.Transform;

namespace Wordplay.View.Transform
{
	static class Transform
	{
		private static AStarSearch<string> SearchAlgorithm;
		private static TransformOptions PreviousOptions;
		private static string WelcomeMessage =
@"Transform one word into another by changing one character at a time.
Valid queries have the form:
	<startWord> <endWord> [-s]
If the -s flag is given, only substitutions will be allowed when transforming words.";

		public static void Run(string dictionaryFilename)
		{
			Console.WriteLine(WelcomeMessage);
			var wordList = GetWordList(dictionaryFilename);

			while (true)
			{
				Console.Write("\nEnter a query ('q' to exit): ");
				string query = Console.ReadLine().Trim().ToLower();

				if (query == "q")
					break;

				string[] queryArgs = query.Split(' ');
				Parser.Default.ParseArguments<TransformOptions>(queryArgs).WithParsed(options =>
				{
					RunSearch(options, wordList);
				});
			}
		}

		private static List<string> GetWordList(string dictionaryFilename)
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

		private static void RunSearch(TransformOptions options, List<string> wordList)
		{
			if (SearchAlgorithm == null || PreviousOptions == null ||
				PreviousOptions.SubstitutionsOnly != options.SubstitutionsOnly)
			{
				Console.WriteLine("Rebuilding word graph...");
				SearchAlgorithm = WordTransformSearchFactory.Create(
					wordList,
					options.End,
					options.SubstitutionsOnly
				);
			}

			PreviousOptions = options;

			Console.WriteLine($"Transforming '{options.Start}' into '{options.End}'...");
			var path = SearchAlgorithm.FindPath(options.Start, options.End).ToList();

			if (path.Count == 0)
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