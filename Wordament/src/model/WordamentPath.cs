using System.Collections.Generic;

using Tools;
using Tools.DataStructures;
using System.Linq;

namespace Wordament.Model
{
	/// <summary>
	/// Represents a word-path pair in the Wordament puzzle grid.
	/// </summary>
	public class WordamentPath
	{
		public readonly string Word;
		public readonly List<GridCell> Locations;
		public readonly int TotalScore;

		internal WordamentPath(IEnumerable<WordSearchState> statePath)
		{
			Validate.IsNotNull(statePath, "statePath");

			Word = statePath.Select(state => state.CurrentString)
				.Aggregate((aggregate, next) =>
				{
					return aggregate + next;
				});

			Locations = new List<GridCell>(statePath.Select(state => state.LastTileAdded.Location));

			int totalTileScore = statePath.Select(state => state.LastTileAdded.Score).Sum();
			TotalScore = (int)(totalTileScore * GetWordMultiplier());
		}

		private double GetWordMultiplier()
		{
			if (Word.Length == 5)
				return 1.5;
			else if (Word.Length == 6 || Word.Length == 7)
				return 2;
			else if (Word.Length >= 8)
				return 2.5;
			else
				return 1;
		}
	}
}
