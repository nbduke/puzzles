using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace SudokuSolver.Model
{
	public class SudokuSolver
	{
		public static Grid<int> Solve(Grid<int> initialValues)
		{
			Validate.IsNotNull(initialValues, "initialValues");
			var puzzleGrid = InitializeVariables(initialValues);
			var solution = RunBacktrackingSearch(puzzleGrid);
			return solution;
		}

		private static SudokuGrid InitializeVariables(Grid<int> initialValues)
		{
			var puzzleGrid = new SudokuGrid(initialValues);

			foreach (var unset in puzzleGrid.GetAllUnsetVariables())
			{
				foreach (var constraint in puzzleGrid.GetSetNeighbors(unset))
				{
					unset.RemovePossibleValue(constraint.Value);
					if (unset.PossibleValuesCount == 0)
						return null;
				}
			}

			return puzzleGrid;
		}

		private static Grid<int> RunBacktrackingSearch(SudokuGrid puzzleGrid)
		{
			var nextVariable = GetNextVariable(puzzleGrid);
			if (nextVariable == null)
				return puzzleGrid.GetValues();

			var constraints = puzzleGrid.GetUnsetNeighbors(nextVariable);

			foreach (int value in nextVariable.GetPossibleValues())
			{
				nextVariable.Value = value;

				if (CheckConsistency(nextVariable, constraints))
				{
					var nextGrid = UpdateGridForAssignment(nextVariable, puzzleGrid);
					var solution = RunBacktrackingSearch(nextGrid);
					if (solution != null)
						return solution;
				}
			}

			nextVariable.Unset();
			return null;
		}

		private static Variable GetNextVariable(SudokuGrid puzzleGrid)
		{
			Variable result = null;
			foreach (var v in puzzleGrid.GetAllUnsetVariables())
			{
				if (result == null || v.PossibleValuesCount < result.PossibleValuesCount)
					result = v;
			}

			return result;
		}

		private static bool CheckConsistency(Variable assigned, IEnumerable<Variable> constraints)
		{
			return constraints.All(v => v.PossibleValuesCount > 1 || !v.IsPossibleValue(assigned.Value));
		}

		private static SudokuGrid UpdateGridForAssignment(Variable assigned, SudokuGrid currentGrid)
		{
			var newGrid = new SudokuGrid(currentGrid);
			foreach (var neighbor in newGrid.GetUnsetNeighbors(assigned))
			{
				neighbor.RemovePossibleValue(assigned.Value);
			}
			return newGrid;
		}
	}
}