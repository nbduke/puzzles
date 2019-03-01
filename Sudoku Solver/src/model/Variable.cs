using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace SudokuSolver.Model
{
	/// <summary>
	/// A variable in the constraint satisfaction problem used to model the
	/// Sudoku puzzle.
	/// </summary>
	/// <remarks>
	/// Each variable has a location, a value (which may not be known), and a
	/// set of possible values that could be assigned to the variable.
	/// </remarks>
	public class Variable
	{
		public const int UNSET_VALUE = 0;
		public const int MAX_POSSIBLE_VALUE = SudokuGrid.GRID_SIZE;

		public readonly GridCell Location;
		public int Value;
		public int PossibleValuesCount { get; private set; }

		private readonly bool[] PossibleValues;

		/// <summary>
		/// Constructs an unset variable at a location in the Sudoku puzzle grid.
		/// </summary>
		/// <param name="location">the location</param>
		public Variable(GridCell location)
			: this(location, UNSET_VALUE)
		{
		}

		/// <summary>
		/// Constructs a variable with a value and location.
		/// </summary>
		/// <param name="location">the location</param>
		/// <param name="value">the value</param>
		public Variable(GridCell location, int value)
		{
			Validate.IsTrue(
				value >= UNSET_VALUE && value <= MAX_POSSIBLE_VALUE,
				"Invalid value"
			);

			Location = location;
			Value = value;
			PossibleValuesCount = MAX_POSSIBLE_VALUE;
			PossibleValues = new bool[MAX_POSSIBLE_VALUE];

			for (int i = 0; i < MAX_POSSIBLE_VALUE; ++i)
			{
				PossibleValues[i] = true;
			}
		}

		/// <summary>
		/// Makes a deep copy of a variable.
		/// </summary>
		/// <param name="other">the variable to copy</param>
		public Variable(Variable other)
		{
			Validate.IsNotNull(other, "other");
			Location = other.Location;
			Value = other.Value;
			PossibleValues = (bool[])other.PossibleValues.Clone();
		}

		public bool IsSet
		{
			get { return Value != UNSET_VALUE; }
		}

		/// <summary>
		/// Returns an enumerable over the possible values remaining for
		/// this variable.
		/// </summary>
		public IEnumerable<int> GetPossibleValues()
		{
			for (int i = 0; i < PossibleValues.Length; ++i)
			{
				if (PossibleValues[i])
					yield return i + 1;
			}
		}

		/// <summary>
		/// Checks whether a value could possibly be assigned to this variable.
		/// </summary>
		/// <param name="value">the value</param>
		/// <returns>true if possible</returns>
		public bool IsPossibleValue(int value)
		{
			Validate.IsTrue(
				value > UNSET_VALUE && value <= MAX_POSSIBLE_VALUE,
				"Invalid value"
			);
			return PossibleValues[value - 1];
		}

		/// <summary>
		/// Removes a value from the set of possible values.
		/// </summary>
		/// <param name="value">the value</param>
		public void RemovePossibleValue(int value)
		{
			if (IsPossibleValue(value))
			{
				PossibleValues[value - 1] = false;
				--PossibleValuesCount;
			}
		}

		/// <summary>
		/// Places the variable in the unset state.
		/// </summary>
		/// <remarks>
		/// This only changes the value, not the set of possible values.
		/// </remarks>
		public void Unset()
		{
			Value = UNSET_VALUE;
		}

		public override bool Equals(object obj)
		{
			return (obj is Variable other)
				&& other.Location.Equals(Location);
		}

		public override int GetHashCode()
		{
			return Location.GetHashCode();
		}
	}
}
