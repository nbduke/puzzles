using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace SudokuSolver.Model
{
	/// <summary>
	/// Solves Sudoku puzzles.
	/// </summary>
	/// <remarks>
	/// The puzzle is modeled as a constraint satisfaction problem with
	/// 81 variables (one for each cell in the grid). A backtracking search
	/// with constraint propagation, arc consistency, and the minimum
	/// remaining values heuristic is employed.
	/// </remarks>
	public class SudokuSolver
	{
		/// <summary>
		/// Solves the Sudoku puzzle represented by a grid of the initial
		/// known and unknown values. 0 represents an unknown value.
		/// </summary>
		/// <param name="initialValues">the initial values of the puzzle</param>
		/// <returns>a grid with all of the values filled in, or null if
		/// a solution could not be found</returns>
		public static Grid<int> Solve(Grid<int> initialValues)
		{
			Validate.IsNotNull(initialValues, "initialValues");
			var puzzleGrid = InitializeVariables(initialValues);
			var solution = RunBacktrackingSearch(puzzleGrid);
			return solution;
		}

		/*
		 * Initialize the possible values for each variable using the known
		 * initial values.
		 */
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
				return puzzleGrid.GetValues(); // the base case: the grid is full

			// Save the constraining variables here to avoid generating them on
			// every iteration below.
			var constraints = puzzleGrid.GetUnsetNeighbors(nextVariable);

			foreach (int value in nextVariable.GetPossibleValues())
			{
				nextVariable.Value = value;

				if (CheckConsistency(nextVariable, constraints))
				{
					var nextGrid = UpdateGridForAssignment(nextVariable, constraints, puzzleGrid);
					var solution = RunBacktrackingSearch(nextGrid);
					if (solution != null)
						return solution;
				}
			}

			nextVariable.Unset();
			return null;
		}

		/*
		 * Returns the unset variable with the fewest remaining possible values.
		 */
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

		/*
		 * Checks if the assigned variable's value is valid by ensuring that, after constraint
		 * propagation, all of its unset neighbors will still have at least one possible value.
		 */
		private static bool CheckConsistency(Variable assigned, IEnumerable<Variable> constraints)
		{
			return constraints.All(v => v.PossibleValuesCount > 1 || !v.IsPossibleValue(assigned.Value));
		}

		/*
		 * Deep copies the grid and propagates the constraints implied by the assigned variable.
		 */
		private static SudokuGrid UpdateGridForAssignment(
			Variable assigned,
			IEnumerable<Variable> constraints,
			SudokuGrid currentGrid)
		{
			var newGrid = new SudokuGrid(currentGrid);
			foreach (var constraint in constraints)
			{
				newGrid[constraint.Location].RemovePossibleValue(assigned.Value);
			}
			return newGrid;
		}
	}
}