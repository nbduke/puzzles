using System.Collections;
using System.Collections.Generic;

using Tools;
using Tools.DataStructures;

namespace Wordplay.Model.Unscramble
{
	/*
	 * Represents a state in the word finding algorithm. A WordScramble puzzle
	 * instance is modeled as a graph whose nodes are letters. There is a directed
	 * edge between two letters only if the concatenation of the two is a prefix
	 * in the dictionary. The word finding algorithm uses backtracking search to
	 * explore the graph, building paths of letters. WordSearchState represents one
	 * such path under construction.
	 */
	class WordSearchState
	{
		public readonly string CurrentString;
		public bool IsWord
		{
			get { return DictionaryNode.IsEndOfWord; }
		}

		private readonly IPrefixTreeNode DictionaryNode;

		public WordSearchState(
			string currentString,
			IPrefixTreeNode dictionaryNode)
		{
			Validate.IsNotNullOrEmpty(currentString);
			Validate.IsNotNull(dictionaryNode, "dictionaryNode");

			CurrentString = currentString;
			DictionaryNode = dictionaryNode;
		}

		public IEnumerable<WordSearchState> GetChildren(
			string scrambledWord,
			BitArray lettersInPath)
		{
			for (int i = 0; i < scrambledWord.Length; ++i)
			{
				if (!lettersInPath[i])
				{
					char nextLetter = scrambledWord[i];
					var nextNode = DictionaryNode.GetChild(nextLetter);

					if (nextNode != null)
					{
						lettersInPath[i] = true;
						yield return new WordSearchState(CurrentString + nextLetter, nextNode);
						lettersInPath[i] = false;
					}
				}
			}
		}
	}
}
