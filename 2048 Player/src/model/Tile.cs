using Tools;
using Tools.DataStructures;

namespace Player.Model
{
	/// <summary>
	/// Represents a position-number pair on the 2048 game grid. Only nonzero
	/// values can be tiles as zero represents an empty cell.
	/// </summary>
	public struct Tile
	{
		public readonly GridCell Cell;
		public readonly int Value;

		public Tile(GridCell cell, int value)
		{
			Validate.IsTrue(value != 0, "A tile cannot represent zero");

			Cell = cell;
			Value = value;
		}

		public int Row
		{
			get
			{
				return Cell.Row;
			}
		}

		public int Column
		{
			get
			{
				return Cell.Column;
			}
		}

		public override bool Equals(object obj)
		{
			return (obj is Tile other) && this == other;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + Cell.GetHashCode();
				hash = hash * 23 + Value.GetHashCode();
				return hash;
			}
		}

		public override string ToString()
		{
			return $"({Row}, {Column}) : {Value}";
		}

		public static bool operator==(Tile a, Tile b)
		{
			return a.Cell == b.Cell && a.Value == b.Value;
		}

		public static bool operator!=(Tile a, Tile b)
		{
			return !(a == b);
		}
	}
}
