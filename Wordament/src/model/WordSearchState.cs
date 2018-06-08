using System.Collections.Generic;

using Tools;
using Tools.DataStructures;

namespace Wordament.Model
{
	/*
	 * Represents a state in the word finding algorithm. A Wordament puzzle
	 * instance is modeled as a graph whose nodes are tiles. There is a directed
	 * edge from tile A to tile B only if A and B are adjacent in the puzzle grid
	 * and the letters from B appended to those from A form a prefix in the
	 * dictionary. The word finding algorithm uses flexible backtracking search to
	 * explore the graph, building paths of tiles. WordSearchState represents one
	 * such under-construction path.
	 */
	class WordSearchState
	{
		public readonly string CurrentString;
		public readonly Tile LastTileAdded;
		public bool IsWord
		{
			get { return DictionaryNode != null && DictionaryNode.IsEndOfWord; }
		}

		private readonly IPrefixTreeNode DictionaryNode;

		public WordSearchState(
			Tile startingTile,
			PrefixTreeDictionary dictionary,
			bool useAlternate = false)
		{
			Validate.IsNotNull(startingTile, "startingTile");
			Validate.IsNotNull(dictionary, "dictionary");

			CurrentString = useAlternate ? startingTile.AlternateLetters : startingTile.Letters;
			LastTileAdded = startingTile;
			DictionaryNode = dictionary.FindNode(CurrentString);
		}

		private WordSearchState(
			string prefix,
			Tile nextTile,
			IPrefixTreeNode nextNode,
			bool useAlternate = false)
		{
			string nextLetters = useAlternate ? nextTile.AlternateLetters : nextTile.Letters;
			CurrentString = prefix + nextLetters;
			LastTileAdded = nextTile;
			DictionaryNode = nextNode;
		}

		public IEnumerable<WordSearchState> GetChildStates(Grid<Tile> grid)
		{
			if (LastTileAdded.Type == TileType.Suffix || DictionaryNode == null)
				yield break; // can't add letters to a suffix or a prefix not in the dictionary

			foreach (var nextTile in grid.GetNeighbors(LastTileAdded.Location))
			{
				if (nextTile.Type == TileType.Prefix)
					continue; // can't add prefixes to the middle of a word

				var nextNode = DictionaryNode.GetDescendant(nextTile.Letters);
				if (nextNode != null)
					yield return new WordSearchState(CurrentString, nextTile, nextNode);

				if (nextTile.Type == TileType.Alternating)
				{
					nextNode = DictionaryNode.GetDescendant(nextTile.AlternateLetters);
					if (nextNode != null)
						yield return new WordSearchState(CurrentString, nextTile, nextNode, true);
				}
			}
		}

		public override bool Equals(object other)
		{
			WordSearchState otherState = other as WordSearchState;
			return otherState != null
				&& LastTileAdded.Location.Equals(otherState.LastTileAdded.Location);
		}

		public override int GetHashCode()
		{
			return LastTileAdded.Location.GetHashCode();
		}
	};
}
