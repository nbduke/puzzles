using System;
using System.IO;

using CommandLine;
using Tools.DataStructures;
using Wordplay.Model.Unscramble;

namespace Wordplay.View.Unscramble
{
	static class Unscramble
	{
		private static string WelcomeMessage =
@"Unscramble a string of letters to find words.
Valid queries have the form:
	<scrambledLetters> [-m <minWordLength>]
If the -m flag is omitted, found words must use all the given letters.";

		public static void Run(string dictionaryFilename)
		{
			Console.WriteLine(WelcomeMessage);
			var dictionary = ParseDictionary(dictionaryFilename);

			while (true)
			{
				Console.Write("\nEnter a query ('q' to exit): ");
				string query = Console.ReadLine().Trim().ToLower();

				if (query == "q")
					break;

				string[] queryArgs = query.Split(' ');
				Parser.Default.ParseArguments<UnscrambleOptions>(queryArgs).WithParsed(options =>
				{
					RunSearch(options, dictionary);
				});
			}
		}

		private static PrefixTreeDictionary ParseDictionary(string dictionaryFilename)
		{
			var dictionary = new PrefixTreeDictionary();
			using (var dictionaryFile = new StreamReader(dictionaryFilename))
			{
				while (!dictionaryFile.EndOfStream)
				{
					string word = dictionaryFile.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(word))
						dictionary.Add(word);
				}
			}

			return dictionary;
		}

		private static void RunSearch(UnscrambleOptions options, PrefixTreeDictionary dictionary)
		{
			if (options.MinWordLength < 0)
				options.MinWordLength = options.Letters.Length;

			Console.WriteLine("Looking for words...");
			var foundWords = WordFinder.FindUnscrambledWords(
				options.Letters,
				options.MinWordLength,
				dictionary
			);

			if (foundWords.Count == 0)
			{
				Console.WriteLine("Sorry, we didn't find any words :(");
			}
			else
			{
				Console.WriteLine($"Done! Found {foundWords.Count} words:");
				foreach (string word in foundWords)
				{
					Console.WriteLine(word);
				}
			}
		}
	}
}