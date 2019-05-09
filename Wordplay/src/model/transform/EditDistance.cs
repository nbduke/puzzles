using System;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	static class EditDistance
	{
		/// <summary>
		/// Calcluates the Hamming distance between two equal-length strings.
		/// </summary>
		public static int CalculateHamming(string a, string b)
		{
			Validate.IsNotNull(a, "a");
			Validate.IsNotNull(b, "b");
			Validate.IsTrue(
				a.Length == b.Length,
				"Hamming distance can only be calculated for strings of equal length"
			);

			int mismatches = 0;
			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != b[i])
					++mismatches;
			}

			return mismatches;
		}

		/// <summary>
		/// Determines whether two strings are separated by a Hamming distance
		/// of at most 1.
		/// </summary>
		/// <remarks>
		/// Attempts to short-cut the full Hamming distance calculation by
		/// returning early if the strings are not Hamming-adjacent.
		/// </remarks>
		/// <returns>true if the strings are Hamming-adjacent.</returns>
		public static bool AreHammingAdjacent(string a, string b)
		{
			Validate.IsNotNull(a, "a");
			Validate.IsNotNull(b, "b");

			if (a.Length != b.Length)
				return false;

			bool hasSeenMismatch = false;
			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != b[i])
				{
					if (hasSeenMismatch)
						return false;
					hasSeenMismatch = true;
				}
			}

			return true;
		}

		/// <summary>
		/// Calculates the Levenshtein distance between two arbitrary strings.
		/// </summary>
		/// <remarks>
		/// This is an implementation of the Needleman-Wunsch algorithm where
		/// insertions, deletions, and substitutions each cost 1.
		/// </remarks>
		public static int CalculateLevenshtein(string a, string b)
		{
			Validate.IsNotNull(a, "a");
			Validate.IsNotNull(b, "b");

			var alignmentGrid = CreateAlignmentGrid(a.Length + 1, b.Length + 1);

			for (int row = 1; row < alignmentGrid.Rows; ++row)
			{
				for (int col = 1; col < alignmentGrid.Columns; ++col)
				{
					int costOfInsertA = alignmentGrid[row - 1, col] + 1;
					int costOfInsertB = alignmentGrid[row, col - 1] + 1;
					int costOfInsertBoth = alignmentGrid[row - 1, col - 1] +
						(a[row - 1] == b[col - 1] ? 0 : 1);

					alignmentGrid[row, col] = Min(
						costOfInsertA,
						costOfInsertB,
						costOfInsertBoth
					);
				}
			}

			return alignmentGrid[alignmentGrid.Rows - 1, alignmentGrid.Columns - 1];
		}

		private static Grid<int> CreateAlignmentGrid(int rows, int columns)
		{
			Grid<int> grid = new Grid<int>(rows, columns);
			for (int row = 0; row < rows; ++row)
			{
				grid[row, 0] = row;
			}
			for (int col = 0; col < columns; ++col)
			{
				grid[0, col] = col;
			}

			return grid;
		}

		private static int Min(params int[] args)
		{
			return args.Min();
		}

		/// <summary>
		/// Determines whether two strings are separated by a Levenshtein distance
		/// of at most 1.
		/// </summary>
		/// <remarks>
		/// Attempts to short-cut the full Levenshtein distance calculation by
		/// returning early if the two strings are not Levenshtein-adjacent.
		/// </remarks>
		/// <returns>true if the strings are Levenshtein-adjacent</returns>
		public static bool AreLevenshteinAdjacent(string a, string b)
		{
			Validate.IsNotNull(a, "a");
			Validate.IsNotNull(b, "b");

			int lengthDifference = Math.Abs(a.Length - b.Length);
			if (lengthDifference > 1)
				return false;
			else if (lengthDifference == 1)
				return AreEqualAfterOneDeletion(a, b);
			else
				return AreHammingAdjacent(a, b);
		}

		/// <summary>
		/// Determines whether the longer string can be converted into the other
		/// only by deleting one letter.
		/// </summary>
		private static bool AreEqualAfterOneDeletion(string a, string b)
		{
			string longer, shorter;
			if (a.Length < b.Length)
			{
				shorter = a;
				longer = b;
			}
			else
			{
				shorter = b;
				longer = a;
			}

			int shortIndex = 0;
			int longIndex = 0;

			while (shortIndex < shorter.Length)
			{
				if (shorter[shortIndex] != longer[longIndex])
				{
					// We've already made one deletion
					if (shortIndex != longIndex)
						return false;
				}
				else
				{
					++shortIndex;
				}

				++longIndex;
			}

			return true;
		}
	}
}