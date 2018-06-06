/*
 * WordFinder.cs
 * 
 * Nathan Duke
 * 3/15/15
 * 
 * Contains the WordFinder static class. This class provides methods for finding words
 * and paths in a Wordament grid.
 */

using System;
using System.Collections.Generic;

using CommonTools.Algorithms.Search;
using CommonTools.DataStructures;

namespace Wordament
{
	public static class WordFinder
	{
		public enum SortOption
		{
			Alphabetical,
			ByWordLengthAscending,
			ByWordLengthDescending,
			ByPathLengthAscending,
			ByPathLengthDescending,
			ByScoreAscending,
			ByScoreDescending
		}

		public static List<string> FindWords(Grid<Tile> grid, int minimumWordLength, PrefixTreeDictionary dictionary)
		{
			var wordPaths = FindWordsAndPaths(grid, minimumWordLength, dictionary);
			List<string> words = new List<string>();
			wordPaths.ForEach((wordPath) => { words.Add(wordPath.Word); });

			return words;
		}

		public static List<WordamentPath> FindWordsAndPaths(
			Grid<Tile> grid,
			int minimumWordLength,
			PrefixTreeDictionary dictionary,
			SortOption resultSortingOption = SortOption.Alphabetical)
		{
			if (grid == null)
				throw new ArgumentNullException("grid");
			else if (dictionary == null || dictionary.Count == 0)
				throw new ArgumentException("The dictionary cannot be empty.");
			else if (minimumWordLength <= 0)
				throw new ArgumentException("minimumWordLength must be at least 1");

			// Create functors used by FlexibleBacktrackingSearch algorithm
			GoalTest<WordBuilder> isValidWord =
				(node) =>
				{
					return node.CumulativePathLength >= minimumWordLength && node.DictionaryHandle.EndOfWord;
				};

			ChildEnumerator<WordBuilder> getChildren =
				(node) =>
				{
					List<WordBuilder> children = new List<WordBuilder>();

					if (node.LastTileAdded.Type == TileType.Suffix)
						return children.GetEnumerator(); // can't add letters to a suffix

					foreach (var neighborTile in grid.GetNeighbors(node.LastTileAdded.Location))
					{
						if (neighborTile.Type == TileType.Prefix)
							continue; // can't add prefixes to the middle of a word

						{
							string nextSubstring = neighborTile.Letters;
							PrefixTreeNode nextHandle = dictionary.PartialLookup(nextSubstring, node.DictionaryHandle);
							if (nextHandle != null)
								children.Add(new WordBuilder(nextHandle, node.Word + nextSubstring, neighborTile, node));
						}

						// If neighbor is an either/or tile, create another child for the other substring
						if (neighborTile.Type == TileType.EitherOr)
						{
							string nextSubstring = neighborTile.OrLetters;
							PrefixTreeNode nextHandle = dictionary.PartialLookup(nextSubstring, node.DictionaryHandle);
							if (nextHandle != null)
								children.Add(new WordBuilder(nextHandle, node.Word + nextSubstring, neighborTile, node));
						}
					}
					return children.GetEnumerator();
				};

			Dictionary<string, WordamentPath> allWordPaths = new Dictionary<string, WordamentPath>();

			GoalAction<WordBuilder> processWord =
				(node) =>
				{
					var locationList = node.GetLocationsOnPath();
					WordamentPath wordPath = new WordamentPath(node.Word, locationList, (int)node.CumulativePathWeight);

					// If we've already found the word, keep the path with the higher score
					if (!allWordPaths.ContainsKey(wordPath.Word) ||
						allWordPaths[wordPath.Word].WordScore < wordPath.WordScore)
					{
						allWordPaths[wordPath.Word] = wordPath;
					}

					return GoalOption.Continue;
				};

			var fbs = new FlexibleBacktrackingSearch<WordBuilder>(isValidWord, getChildren, processWord);

			// Run FindAllWords algorithm starting from every tile in the grid
			foreach (var tile in grid.Flatten(GridOrder.RowMajor))
			{
				{
					string prefix = tile.Letters;
					PrefixTreeNode startingHandle = dictionary.PartialLookup(prefix);

					if (startingHandle != null)
					{
						WordBuilder startingNode = new WordBuilder(startingHandle, prefix, tile);
						fbs.Search(startingNode, (uint)grid.Cells);
					}
				}

				// If tile is an either/or tile, start a new search using the other string
				if (tile.Type == TileType.EitherOr)
				{
					string prefix = tile.OrLetters;
					PrefixTreeNode startingHandle = dictionary.PartialLookup(prefix);

					if (startingHandle != null)
					{
						WordBuilder startingNode = new WordBuilder(startingHandle, prefix, tile);
						fbs.Search(startingNode, (uint)grid.Cells);
					}
				}
			}

			List<WordamentPath> result = new List<WordamentPath>(allWordPaths.Values);
			SortPaths(result, resultSortingOption);
			return result;
		}

		private static void SortPaths(List<WordamentPath> paths, SortOption sortOption)
		{
			switch (sortOption)
			{
				case SortOption.Alphabetical:
					paths.Sort((a, b) => a.Word.CompareTo(b.Word));
					break;
				case SortOption.ByScoreAscending:
					paths.Sort((a, b) => a.WordScore.CompareTo(b.WordScore));
					break;
				case SortOption.ByScoreDescending:
					paths.Sort((a, b) => -a.WordScore.CompareTo(b.WordScore));
					break;
				case SortOption.ByWordLengthAscending:
					paths.Sort((a, b) => a.Word.Length.CompareTo(b.Word.Length));
					break;
				case SortOption.ByWordLengthDescending:
					paths.Sort((a, b) => -a.Word.Length.CompareTo(b.Word.Length));
					break;
				case SortOption.ByPathLengthAscending:
					paths.Sort((a, b) => a.Path.Count.CompareTo(b.Path.Count));
					break;
				case SortOption.ByPathLengthDescending:
					paths.Sort((a, b) => -a.Path.Count.CompareTo(b.Path.Count));
					break;
				default:
					throw new ArgumentException("Unrecognized SortOption value");
			}
		}
	}
}
