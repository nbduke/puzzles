using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Algorithms.Search;
using Tools.DataStructures;

namespace Wordplay.Model.Unscramble
{
	/// <summary>
	/// Encapsulates a WordScramble puzzle instance and exposes a method for finding
	/// all words that can be formed from letters in the scrambled word.
	/// </summary>
	public class WordFinder
	{
		private readonly string ScrambledWord;
		private readonly int MinWordLength;
		private readonly PrefixTreeDictionary Dictionary;

		public static List<string> FindUnscrambledWords(
			string scrambledWord,
			int minWordLength,
			PrefixTreeDictionary dictionary)
		{
			var wordFinder = new WordFinder(
				scrambledWord,
				minWordLength,
				dictionary);

			return wordFinder.FindAllWords();
		}

		public WordFinder(
			string scrambledWord,
			int minWordLength,
			PrefixTreeDictionary dictionary)
		{
			Validate.IsNotNullOrEmpty(scrambledWord);
			Validate.IsNotNull(dictionary, "dictionary");
			Validate.IsTrue(minWordLength >= 1 && minWordLength <= scrambledWord.Length,
				"Minimum word length must be at least 1 and at most the length of the input string.");

			ScrambledWord = scrambledWord;
			MinWordLength = minWordLength;
			Dictionary = dictionary;
		}

		public List<string> FindAllWords()
		{
			var words = new SortedSet<string>();
			var lettersInPath = new BitArray(ScrambledWord.Length);

			var searcher = new FlexibleBacktrackingSearch<WordSearchState>(
				state => state.GetChildren(ScrambledWord, lettersInPath),
				true);

			for (int i = 0; i < ScrambledWord.Length; ++i)
			{
				string initialString = ScrambledWord[i].ToString();
				var initialNode = Dictionary.FindNode(initialString);

				if (initialNode != null)
				{
					lettersInPath[i] = true;
					searcher.Search(
						new WordSearchState(initialString, initialNode),
						node =>
						{
							if (node.State.IsWord &&
								node.State.CurrentString.Length >= MinWordLength)
							{
								words.Add(node.State.CurrentString);
							}
							return NodeOption.Continue;
						}
					);
					lettersInPath[i] = false;
				}
			}

			return words.ToList();
		}
	}
}
