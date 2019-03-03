using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace SudokuSolver.Model
{
	/// <summary>
	/// Represents a standard Sudoku puzzle grid. Each cell contains a Variable
	/// object which will be used to solve the puzzle.
	/// </summary>
	public class SudokuGrid
	{
		public const int GRID_SIZE = 9;
		public const int SQUARE_SIZE = 3;

		private readonly Grid<Variable> Variables;

		/// <summary>
		/// Constructs a Sudoku puzzle from a grid of numbers representing the
		/// known and unknown initial values.
		/// </summary>
		/// <param name="initialValues">the grid of initial values</param>
		public SudokuGrid(Grid<int> initialValues)
		{
			Validate.IsNotNull(initialValues, "initialValues");
			Variables = new Grid<Variable>(GRID_SIZE, GRID_SIZE);

			foreach (var itemAndLocation in initialValues.GetItemLocationPairs())
			{
				var location = itemAndLocation.Key;
				Variables[location] = new Variable(location, itemAndLocation.Value);
			}
		}

		/// <summary>
		/// Makes a deep copy of a grid.
		/// </summary>
		/// <param name="other">the grid to copy</param>
		public SudokuGrid(SudokuGrid other)
		{
			Validate.IsNotNull(other, "other");
			Variables = new Grid<Variable>(GRID_SIZE, GRID_SIZE);

			foreach (var v in other.Variables)
			{
				Variables[v.Location] = new Variable(v);
			}
		}

		public Variable this[GridCell location]
		{
			get { return Variables[location]; }
		}

		public Variable this[int row, int column]
		{
			get { return Variables[row, column]; }
		}

		/// <summary>
		/// Returns the numerical values within the grid.
		/// </summary>
		public Grid<int> GetValues()
		{
			var values = new Grid<int>(GRID_SIZE, GRID_SIZE);
			foreach (var v in Variables)
			{
				values[v.Location] = v.Value;
			}
			return values;
		}

		/// <summary>
		/// Returns all unset variables in the grid.
		/// </summary>
		public IEnumerable<Variable> GetAllUnsetVariables()
		{
			return Variables.Where(v => !v.IsSet);
		}

		/// <summary>
		/// Returns the neighboring variables (those in the same row, column, or
		/// square) that are unset.
		/// </summary>
		/// <param name="variable">the variable whose neighbors will be returned</param>
		public IEnumerable<Variable> GetUnsetNeighbors(Variable variable)
		{
			Validate.IsNotNull(variable, "variable");
			return GetNeighbors(variable).Where(v => !v.IsSet);
		}

		/// <summary>
		/// Returns the neighboring variables (those in the same row, column, or
		/// square) that are set.
		/// </summary>
		/// <param name="variable">the variable whose neighbors will be returned</param>
		public IEnumerable<Variable> GetSetNeighbors(Variable variable)
		{
			Validate.IsNotNull(variable, "variable");
			return GetNeighbors(variable).Where(v => v.IsSet);
		}

		private IEnumerable<Variable> GetNeighbors(Variable variable)
		{
			var neighborsWithDuplicates =
				Variables.RowAt(variable.Location.Row)
				.Concat(Variables.ColumnAt(variable.Location.Column))
				.Concat(GetVariablesInSquare(variable.Location));

			var uniqueNeighbors = new HashSet<Variable>(neighborsWithDuplicates);
			uniqueNeighbors.Remove(variable);

			return uniqueNeighbors;
		}

		private IEnumerable<Variable> GetVariablesInSquare(GridCell cell)
		{
			int rowOfTopLeft = (cell.Row / SQUARE_SIZE) * SQUARE_SIZE;
			int columnOfTopLeft = (cell.Column / SQUARE_SIZE) * SQUARE_SIZE;

			for (int row = 0; row < SQUARE_SIZE; ++row)
			{
				for (int column = 0; column < SQUARE_SIZE; ++column)
				{
					Variable v = Variables[rowOfTopLeft + row, columnOfTopLeft + column];
					yield return v;
				}
			}
		}
	}
}