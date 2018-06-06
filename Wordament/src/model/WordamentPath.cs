/*
 * WordamentPath.cs
 * 
 * Nathan Duke
 * 3/15/15
 * 
 * Contains the WordamentPath class, which represents a unique path of Tiles
 * on the Wordament grid. The path is represented by a list of GridCells and
 * a string which is the concatenation of all the Tile strings.
 */

using System.Collections.Generic;

using CommonTools.DataStructures;

namespace Wordament
{
	public class WordamentPath
	{
		public string Word { get; private set; }
		public List<GridCell> Path { get; private set; }
		public int TotalTileScore { get; private set; }
		public int WordScore { get; private set; }

		public WordamentPath(string word, List<GridCell> path, int totalTileScore)
		{
			Word = word;
			Path = path;
			TotalTileScore = totalTileScore;
			ApplyWordMultiplier();
		}

		private void ApplyWordMultiplier()
		{
			if (Word.Length == 5)
				WordScore = (int)(TotalTileScore * 1.5);
			else if (Word.Length == 6 || Word.Length == 7)
				WordScore = TotalTileScore * 2;
			else if (Word.Length >= 8)
				WordScore = (int)(TotalTileScore * 2.5);
			else
				WordScore = TotalTileScore;
		}
	}
}
