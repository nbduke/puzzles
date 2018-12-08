using System;
using System.Collections.Generic;
using System.Linq;

using Tools.DataStructures;
using Tools.Math;

namespace Player.Model
{
	/// <summary>
	/// Represents a state of the 2048 game. A state is a particular arrangement of
	/// numbers within the 4x4 grid. The class exposes methods for manipulating and
	/// querying the state.
	/// </summary>
	public class GameState
	{
		public const int DEFAULT_GOAL = 2048;
		public const int GRID_SIZE = 4;

		public readonly int GoalNumber;
		public int HighestNumber { get; private set; }
		public int FilledCells { get; private set; }

		internal readonly Grid<int> Grid;

		/// <summary>
		/// Constructs a GameState with a number that defines the winning condition.
		/// </summary>
		/// <param name="goalNumber">the goal number (default is 2048)</param>
		public GameState(int goalNumber = DEFAULT_GOAL)
		{
			GoalNumber = goalNumber;
			HighestNumber = 0;
			FilledCells = 0;
			Grid = new Grid<int>(GRID_SIZE, GRID_SIZE, 0);
		}

		/// <summary>
		/// Constructs a GameState by copying another one.
		/// </summary>
		/// <param name="other">the state to copy</param>
		public GameState(GameState other)
		{
			GoalNumber = other.GoalNumber;
			HighestNumber = other.HighestNumber;
			FilledCells = other.FilledCells;
			Grid = new Grid<int>(other.Grid);
		}

		/// <summary>
		/// Gets the number at the specified row and column.
		/// </summary>
		public int this[int row, int column]
		{
			get
			{
				return Grid[row, column];
			}
		}

		/// <summary>
		/// Gets the number at the specified cell.
		/// </summary>
		public int this[GridCell cell]
		{
			get
			{
				return Grid[cell];
			}
		}

		/// <summary>
		/// Returns true if this GameState is a winning state.
		/// </summary>
		public bool IsWin
		{
			get
			{
				return HighestNumber >= GoalNumber;
			}
		}

		/// <summary>
		/// Returns true if this GameState is a losing state.
		/// </summary>
		public bool IsLoss
		{
			get
			{
				return !IsWin
					&& IsFull
					&& GetLegalActions().Count() == 0;
			}
		}

		/// <summary>
		/// Returns true if the grid is full, i.e. there are no empty cells.
		/// </summary>
		public bool IsFull
		{
			get
			{
				return EmptyCells == 0;
			}
		}

		/// <summary>
		/// Returns the number of empty cells in the grid.
		/// </summary>
		public int EmptyCells
		{
			get
			{
				return Grid.Cells - FilledCells;
			}
		}

		/// <summary>
		/// Determines whether a cell is empty. A cell is considered to be empty
		/// if its value is 0.
		/// </summary>
		/// <param name="cell">the cell</param>
		/// <returns>true if the cell is empty</returns>
		public bool IsCellEmpty(GridCell cell)
		{
			return IsCellEmpty(cell.Row, cell.Column);
		}

		/// <summary>
		/// Determines whether a cell is empty. A cell is considered to be empty
		/// if its value is 0.
		/// </summary>
		/// <param name="row">the row</param>
		/// <param name="column">the column</param>
		/// <returns>true if the cell is empty</returns>
		public bool IsCellEmpty(int row, int column)
		{
			return Grid[row, column] == 0;
		}

		/// <summary>
		/// Returns an enumerable over the tiles in the grid. A tile is a nonempty
		/// position and its value.
		/// </summary>
		public IEnumerable<Tile> GetTiles()
		{
			foreach (var pair in Grid.GetItemLocationPairs())
			{
				if (pair.Value != 0)
					yield return new Tile(pair.Key, pair.Value);
			}
		}

		/// <summary>
		/// Returns an enumerable over the empty cells in the grid.
		/// </summary>
		public IEnumerable<GridCell> GetEmptyCells()
		{
			foreach (var pair in Grid.GetItemLocationPairs())
			{
				if (pair.Value == 0)
					yield return pair.Key;
			}
		}

		/// <summary>
		/// Returns an enumerable of Actions that are legal in the current state, if any.
		/// </summary>
		public IEnumerable<Action> GetLegalActions()
		{
			foreach (Action a in Enum.GetValues(typeof(Action)))
			{
				if (IsActionLegal(a))
					yield return a;
			}
		}

		/// <summary>
		/// Determines whether an action is legal.
		/// </summary>
		/// <remarks>
		/// `NoAction` is never legal, and there are no legal actions in a winning state.
		/// </remarks>
		/// <param name="a">the action</param>
		/// <returns>true if the action is legal</returns>
		public bool IsActionLegal(Action a)
		{
			if (a == Action.NoAction || IsWin)
				return false;

			for (int i = 0; i < GRID_SIZE; ++i)
			{
				int previousNumber = 0;
				bool hasSeenNonemptyCell = false;

				foreach (int number in GetSequenceInDirection(i, a))
				{
					// An action is legal if two numbers can be combined in the direction
					// of the action (i.e. they're equal) or there is an empty cell that
					// is preceded by at least one nonempty cell
					if ((number != 0 && number == previousNumber) ||
						(number == 0 && hasSeenNonemptyCell))
					{
						return true;
					}
					else
					{
						previousNumber = number;
					}

					hasSeenNonemptyCell = hasSeenNonemptyCell || number > 0;
				}
			}

			return false;
		}

		/// <summary>
		/// Applies an action to this GameState, if legal, causing the numbers to be
		/// shifted accordingly.
		/// </summary>
		/// <param name="a">the action</param>
		/// <returns>true if the action is legal</returns>
		public bool DoAction(Action a)
		{
			if (IsActionLegal(a))
			{
				for (int i = 0; i < GRID_SIZE; ++i)
				{
					var condensedSequence = CondenseAndShift(GetSequenceInDirection(i, a));
					SetSequenceInDirection(condensedSequence, i, a);
				}

				UpdateStatistics();

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds a tile to the grid if its position is not already occupied.
		/// </summary>
		/// <param name="t">the tile</param>
		/// <returns>true if the tile was added</returns>
		public bool AddTile(Tile t)
		{
			if (IsCellEmpty(t.Cell))
			{
				Grid[t.Cell] = t.Value;
				++FilledCells;

				if (t.Value > HighestNumber)
					HighestNumber = t.Value;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Adds a tile to one of the empty cells on the grid, if any.
		/// </summary>
		/// <remarks>
		/// The tile's position is chosen uniformly at random from the set of empty
		/// cells. The tile's value is 2 with the given probability and 4 otherwise.
		/// </remarks>
		/// <param name="probability">the probability of placing a 2</param>
		/// <returns>true if a tile was added</returns>
		public bool AddRandomTile(double probability)
		{
			if (!IsFull)
			{
				var emptyCells = new List<GridCell>(GetEmptyCells());
				GridCell cell = RandomProvider.Select(emptyCells);
				int number = RandomProvider.FlipCoin(probability) ? 2 : 4;
				var tile = new Tile(cell, number);

				return AddTile(tile);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Removes the tile at the given cell, if one exists.
		/// </summary>
		/// <param name="cell">the cell</param>
		/// <returns>the value of the cell, if occupied, or zero</returns>
		public int RemoveTile(GridCell cell)
		{
			return RemoveTile(cell.Row, cell.Column);
		}

		/// <summary>
		/// Removes the tile at the given row and column, if one exists.
		/// </summary>
		/// <param name="row">the row</param>
		/// <param name="column">the column</param>
		/// <returns></returns>
		public int RemoveTile(int row, int column)
		{
			int number = Grid[row, column];
			if (number != 0)
			{
				Grid[row, column] = 0;
				--FilledCells;

				if (number == HighestNumber)
					UpdateStatistics();

			}

			return number;
		}

		public override bool Equals(object obj)
		{
			return (obj is GameState other) &&
				GoalNumber == other.GoalNumber &&
				Grid.SequenceEqual(other.Grid);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 13;
				foreach (Tile t in GetTiles())
				{
					hash = hash * 29 + t.GetHashCode();
				}

				return hash;
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			for (int row = 0; row < GRID_SIZE; ++row)
			{
				foreach (int number in Grid.RowAt(row))
				{
					result += $"{number}  ";
				}
				result += "\n";
			}

			return result;
		}

		/*
		 * Updates statistics of the GameState.
		 */
		private void UpdateStatistics()
		{
			FilledCells = 0;
			HighestNumber = 0;

			foreach (int number in Grid)
			{
				if (number != 0)
				{
					++FilledCells;
					if (number > HighestNumber)
						HighestNumber = number;
				}
			}
		}

		/*
		 * Returns the sequence of numbers on the grid in a given direction. Note that
		 * index may be either a row index or a column index depending on direction.
		 */
		private IEnumerable<int> GetSequenceInDirection(int index, Action direction)
		{
			switch (direction)
			{
				case Action.Down:
					return Grid.ColumnAt(index);

				case Action.Up:
					return Grid.ColumnAt(index).Reverse();

				case Action.Right:
					return Grid.RowAt(index);

				case Action.Left:
					return Grid.RowAt(index).Reverse();

				default:
					return new int[] { };
			}
		}

		/*
		 * Sets a sequence of numbers on the grid in the given direction. Note that
		 * index may be either a row index or a column index depending on direction.
		 */
		private void SetSequenceInDirection(
			List<int> sequence,
			int index,
			Action direction)
		{
			int i = 0;
			switch (direction)
			{
				case Action.Down:
					for (int row = 0; row < GRID_SIZE; ++row)
					{
						Grid[row, index] = sequence[i++];
					}
					break;

				case Action.Up:
					for (int row = GRID_SIZE - 1; row >= 0 ; --row)
					{
						Grid[row, index] = sequence[i++];
					}
					break;

				case Action.Right:
					for (int column = 0; column < GRID_SIZE; ++column)
					{
						Grid[index, column] = sequence[i++];
					}
					break;

				case Action.Left:
					for (int column = GRID_SIZE - 1; column >= 0; --column)
					{
						Grid[index, column] = sequence[i++];
					}
					break;

				default:
					break;
			}
		}

		/*
		 * Condenses a sequence from the grid by adding duplicate numbers that are
		 * adjacent or separated by empty cells.
		 */
		private List<int> CondenseAndShift(IEnumerable<int> sequence)
		{
			List<int> result = new List<int>(GRID_SIZE);
			int previousNumber = 0;

			foreach (int number in sequence)
			{
				if (number > 0)
				{
					if (number == previousNumber)
					{
						result[result.Count - 1] = number * 2;
						previousNumber = 0;
					}
					else
					{
						result.Add(number);
						previousNumber = number;
					}
				}
			}

			// Prepend zeros to fill the list
			for (int i = result.Count; i < GRID_SIZE; ++i)
			{
				result.Insert(0, 0);
			}

			return result;
		}
	}
}
