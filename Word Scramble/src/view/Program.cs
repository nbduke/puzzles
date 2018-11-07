using System;
using System.Collections.Generic;
using System.IO;

using Tools.DataStructures;
using WordScramble.Model;

namespace WordScramble.View
{
	class Program
	{
		const string ProgramName = "WordScramble";
		const int NumRequiredArgs = 3;

		static void Main(string[] args)
		{
			if (args.Length != NumRequiredArgs)
			{
				HelpMessage();
				return;
			}

			try
			{
				string scrambledWord = args[0].ToLower();
				int minimumWordLength = int.Parse(args[1]);
				string dictFileName = args[2];

				var dictionary = BuildDictionaryFromFile(dictFileName);
				GenerateOutput(WordFinder.FindUnscrambledWords(scrambledWord, minimumWordLength, dictionary));
			}
			catch (FormatException)
			{
				ErrorMessage("The provided minimum word length could not be parsed as an integer.");
			}
			catch (Exception otherError)
			{
				ErrorMessage(otherError.Message);
			}
		}

		static void GenerateOutput(List<string> foundWords)
		{
			Console.WriteLine("---------------------------------------------------");
			Console.WriteLine("\t\tFound {0} words:", foundWords.Count);
			Console.WriteLine("---------------------------------------------------");
			foundWords.ForEach((word) => { Console.WriteLine(word); });
		}

		static PrefixTreeDictionary BuildDictionaryFromFile(string filename)
		{
			PrefixTreeDictionary dictionary = new PrefixTreeDictionary();
			using (StreamReader inputFile = new StreamReader(filename))
			{
				while (!inputFile.EndOfStream)
				{
					string word = inputFile.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(word))
						dictionary.Add(word);
				}
			}

			return dictionary;
		}

		static void HelpMessage()
		{
			Console.WriteLine("Usage: " + ProgramName + " <input string> <minimum word length> <dictionary file>");
		}

		static void ErrorMessage(string message)
		{
			Console.WriteLine("-------------------- ERROR --------------------");
			Console.WriteLine(message);
			Console.WriteLine("Terminating.");
		}
	}
}
