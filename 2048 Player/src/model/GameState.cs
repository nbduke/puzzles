using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public const int GRID_SIZE = 4;

		public readonly int GoalNumber;
		public int HighestNumber { get; private set; }
		public int FilledCells { get; private set; }

		internal readonly Grid<int> Grid;

		/// <summary>
		/// Constructs a GameState with a number that defines the winning condition.
		/// </summary>
		/// <param name="goalNumber">the goal number (default is 2048)</param>
		public GameState(int goalNumber = Constants.DEFAULT_GOAL)
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
			if (IsActionLegal(Action.Left))
				yield return Action.Left;
			if (IsActionLegal(Action.Up))
				yield return Action.Up;
			if (IsActionLegal(Action.Right))
				yield return Action.Right;
			if (IsActionLegal(Action.Down))
				yield return Action.Down;
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
		/// Applies an action to this GameState, updating the grid as needed.
		/// </summary>
		/// <remarks>
		/// If `a` is not legal, the behavior of this method is undefined.
		/// </remarks>
		/// <param name="a">the action</param>
		public void ApplyAction(Action a)
		{
			if (a != Action.NoAction)
			{
				for (int i = 0; i < GRID_SIZE; ++i)
				{
					var condensedSequence = CondenseAndShift(GetSequenceInDirection(i, a));
					SetSequenceInDirection(condensedSequence, i, a);
				}

				UpdateStatistics();
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
		/// Adds a tile to one of the empty cells on the grid. If no cells are empty,
		/// this method throws InvalidOperationException.
		/// </summary>
		/// <remarks>
		/// The tile's position is chosen uniformly at random from the set of empty
		/// cells. The tile's value is chosen with a weighted coin flip.
		/// </remarks>
		/// <returns>the tile that was added</returns>
		public Tile AddRandomTile()
		{
			if (IsFull)
				throw new InvalidOperationException("There are no empty cells to place a tile in.");

			var emptyCells = new List<GridCell>(GetEmptyCells());
			GridCell cell = RandomProvider.Select(emptyCells);
			int number = RandomProvider.FlipCoin(Constants.TILE_PROBABILITY_2) ? 2 : 4;
			var tile = new Tile(cell, number);
			AddTile(tile);

			return tile;
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
			var stringBuilder = new StringBuilder();
			for (int row = 0; row < GRID_SIZE; ++row)
			{
				foreach (int number in Grid.RowAt(row))
				{
					stringBuilder.Append($"{number}  ");
				}
				stringBuilder.AppendLine();
			}

			return stringBuilder.ToString();
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
