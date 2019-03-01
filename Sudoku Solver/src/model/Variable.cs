using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.DataStructures;

namespace SudokuSolver.Model
{
	public class Variable
	{
		public const int UNSET_VALUE = 0;
		public const int MAX_POSSIBLE_VALUE = SudokuGrid.GRID_SIZE;

		public readonly GridCell Location;
		public int Value;
		public int PossibleValuesCount { get; private set; }

		private readonly bool[] PossibleValues;

		public Variable(GridCell location)
			: this(location, UNSET_VALUE)
		{
		}

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

		public IEnumerable<int> GetPossibleValues()
		{
			for (int i = 0; i < PossibleValues.Length; ++i)
			{
				if (PossibleValues[i])
					yield return i + 1;
			}
		}

		public bool IsPossibleValue(int value)
		{
			Validate.IsTrue(
				value > UNSET_VALUE && value <= MAX_POSSIBLE_VALUE,
				"Invalid value"
			);
			return PossibleValues[value - 1];
		}

		public void RemovePossibleValue(int value)
		{
			if (IsPossibleValue(value))
			{
				PossibleValues[value - 1] = false;
				--PossibleValuesCount;
			}
		}

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
