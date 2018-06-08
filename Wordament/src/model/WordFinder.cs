using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Algorithms.Search;
using Tools.DataStructures;

namespace Wordament.Model
{
	/// <summary>
	/// Encapsulates a Wordament puzzle instance and exposes a method for finding
	/// all words in the grid along with their paths.
	/// </summary>
	public class WordFinder
	{
		private readonly PrefixTreeDictionary Dictionary;
		private readonly Grid<Tile> PuzzleGrid;
		private readonly Dictionary<string, WordamentPath> AllWordPaths;
		private uint MinimumWordLength;

		public WordFinder(PrefixTreeDictionary dictionary, Grid<Tile> puzzleGrid)
		{
			Validate.IsNotNull(dictionary, "dictionary");
			Validate.IsNotNull(puzzleGrid, "puzzleGrid");

			Dictionary = dictionary;
			PuzzleGrid = puzzleGrid;
			AllWordPaths = new Dictionary<string, WordamentPath>();
		}

		/// <summary>
		/// Computes all words in the puzzle grid. Returns a list of unique
		/// word-paths in alphabetical order.
		/// </summary>
		/// <param name="minimumWordLength">the minimum length for a word to count</param>
		public List<WordamentPath> FindWordsWithPaths(uint minimumWordLength = 1)
		{
			AllWordPaths.Clear();
			MinimumWordLength = minimumWordLength;

			var stateSearcher = new FlexibleBacktrackingSearch<WordSearchState>(state =>
			{
				return state.GetChildStates(PuzzleGrid);
			});

			// Run flexible backtracking search starting at each tile in the grid.
			foreach (var tile in PuzzleGrid)
			{
				SearchStartingAtTile(tile, stateSearcher, node => ProcessSearchNode(node));

				if (tile.Type == TileType.Alternating)
				{
					SearchStartingAtTile(tile, stateSearcher, node => ProcessSearchNode(node), true);
				}
			}

			return new List<WordamentPath>(AllWordPaths.Values);
		}

		/*
		 * Determines whether a node terminates a valid word. If it does, the word-path
		 * is recorded.
		 */
		private NodeOption ProcessSearchNode(PathNode<WordSearchState> node)
		{
			var state = node.State;
			if (state.IsWord && (uint)state.CurrentString.Length >= MinimumWordLength)
			{
				var wordPath = new WordamentPath(node.GetPath());

				// If we've already found the word, keep the path with the higher score
				if (!AllWordPaths.TryGetValue(wordPath.Word, out WordamentPath matchingPath) ||
					matchingPath.TotalScore < wordPath.TotalScore)
				{
					AllWordPaths[wordPath.Word] = wordPath;
				}
			}

			return NodeOption.Continue;
		}

		private void SearchStartingAtTile(
			Tile tile,
			FlexibleBacktrackingSearch<WordSearchState> stateSearcher,
			NodeAction<WordSearchState> nodeAction,
			bool useAlternate = false)
		{
			var startingState = new WordSearchState(tile, Dictionary, useAlternate);
			stateSearcher.Search(startingState, nodeAction);
		}
	};
}
