/*
 * WordBuilder.cs
 * 
 * Nathan Duke
 * 3/15/15
 * 
 * Contains the WordBuilder class. WordBuilder extends PathNodeBase and is thus
 * used to encapsulate state in the FlexibleBacktrackingSearch algorithm.
 */

using System.Collections.Generic;

using CommonTools.Algorithms.Search;
using CommonTools.DataStructures;

namespace Wordament
{
	public class WordBuilder : PathNodeBase
	{
		public PrefixTreeNode DictionaryHandle { get; private set; }
		public string Word { get; private set; }
		public Tile LastTileAdded { get; private set; }

		public WordBuilder(
			PrefixTreeNode handle,
			string prefix,
			Tile startingTile)
			: this(handle, prefix, startingTile, null)
		{
		}

		public WordBuilder(
			PrefixTreeNode handle,
			string word,
			Tile lastTileAdded,
			WordBuilder parent)
			: base(parent, lastTileAdded.Score)
		{
			DictionaryHandle = handle;
			Word = word;
			LastTileAdded = lastTileAdded;
		}

		public override bool Equals(object node)
		{
			return LastTileAdded.Location == ((WordBuilder)node).LastTileAdded.Location;
		}

		public override int GetHashCode()
		{
			return LastTileAdded.Location.GetHashCode();
		}

		public List<Tile> GetTilesOnPath()
		{
			List<Tile> result = new List<Tile>();
			List<PathNodeBase> nodePath = new List<PathNodeBase>(GetPathToRoot());
			
			for (int i = nodePath.Count - 1; i >= 0; --i)
			{
				result.Add(((WordBuilder)nodePath[i]).LastTileAdded);
			}

			return result;
		}

		public List<GridCell> GetLocationsOnPath()
		{
			List<GridCell> result = new List<GridCell>();
			List<PathNodeBase> nodePath = new List<PathNodeBase>(GetPathToRoot());

			for (int i = nodePath.Count - 1; i >= 0; --i)
			{
				result.Add(((WordBuilder)nodePath[i]).LastTileAdded.Location);
			}

			return result;
		}
	}
}
