using System.Linq;

using Tools;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	static class EditDistance
	{
		public static int CountMismatches(string a, string b)
		{
			Validate.IsNotNullOrEmpty(a);
			Validate.IsNotNullOrEmpty(b);

			if (a.Length != b.Length)
				return int.MaxValue;

			int mismatches = 0;
			for (int i = 0; i < a.Length; ++i)
			{
				if (a[i] != b[i])
					++mismatches;
			}

			return mismatches;
		}

		public static int Calculate(string a, string b)
		{
			Validate.IsNotNullOrEmpty(a);
			Validate.IsNotNullOrEmpty(b);

			var alignmentGrid = CreateAlignmentGrid(a.Length + 1, b.Length + 1);

			for (int row = 1; row < alignmentGrid.Rows; ++row)
			{
				for (int col = 1; col < alignmentGrid.Columns; ++col)
				{
					int costOfInsertA = alignmentGrid[row - 1, col] + 1;
					int costOfInsertB = alignmentGrid[row, col - 1] + 1;
					int costOfInsertBoth = alignmentGrid[row - 1, col - 1] +
						a[row - 1] == b[col - 1] ? 0 : 1;

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
	}
}