using System.Collections.Generic;
using System.Linq;

using Tools.DataStructures;
using Tools.Math;

namespace Player.Model
{
	/// <summary>
	/// Applies the Expectimax algorithm to play the 2048 game.
	/// </summary>
	public class ExpectimaxPlayer : IGamePlayer
	{
		private const double NO_VALUE = double.MinValue;

		/// <summary>
		/// Returns the best action to take in a game state using a smart depth
		/// limit on the search. If no actions are legal, NoAction is returned.
		/// </summary>
		/// <param name="state">the game state</param>
		public Action GetPolicy(GameState state)
		{
			return GetPolicy(state, new DepthLimit(state));
		}

		/// <summary>
		/// Returns the best action to take in a game state using the given depth limit.
		/// If no actions are legal, NoAction is returned.
		/// </summary>
		/// <param name="state">the game state</param>
		/// <param name="searchLimit">the depth limit</param>
		public Action GetPolicy(GameState state, IDepthLimit searchLimit)
		{
			return GetPolicies(state, searchLimit).Best();
		}

		/// <summary>
		/// Returns the list of legal actions in a game state with their corresponding
		/// expected values.
		/// </summary>
		/// <param name="state">the game state</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state)
		{
			return GetPolicies(state, new DepthLimit(state));
		}

		/// <summary>
		/// Returns the list of legal actions in a game state with their corresponding
		/// expected values.
		/// </summary>
		/// <param name="state">the game state</param>
		/// <param name="searchLimit">a depth limit for the search algorithm</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state, IDepthLimit searchLimit)
		{
			foreach (var action in ShuffledLegalActions(state))
			{
				searchLimit.IncreaseDepth();
				double value = ExpectedValue(new GameState(state), action, searchLimit);
				searchLimit.DecreaseDepth();

				yield return new ActionValue()
				{
					Action = action,
					Value = value
				};
			}
		}

		/*
		 * Calculates the expected value over all possible states that could result from
		 * taking the given action in the given state.
		 */
		private double ExpectedValue(GameState state, Action action, IDepthLimit searchLimit)
		{
			state.ApplyAction(action);
			double placementProbability = 1.0 / state.EmptyCells;
			double probability2 = placementProbability * Constants.TILE_PROBABILITY_2;
			double probability4 = placementProbability * Constants.TILE_PROBABILITY_4;
			double expectedValue = 0;

			// Tries all possible placements of '2' and '4' tiles, calculating the
			// value of each outcome and aggregating values into the expected value.
			foreach (var cell in state.GetEmptyCells())
			{
				var tile2 = new Tile(cell, 2);
				state.AddTile(tile2);
				expectedValue += MaxValue(state, searchLimit) * probability2;
				state.RemoveTile(cell);

				var tile4 = new Tile(cell, 4);
				state.AddTile(tile4);
				expectedValue += MaxValue(state, searchLimit) * probability4;
				state.RemoveTile(cell);
			}

			return expectedValue;
		}

		/*
		 * Returns the maximum value over all states reachable from the given state.
		 */
		private double MaxValue(GameState state, IDepthLimit searchLimit)
		{
			if (state.IsWin)
				return 100.0;
			else if (state.IsLoss)
				return 0;
			else if (searchLimit.Done())
				return Evaluate(state);

			double maxValue = NO_VALUE;
			foreach (var action in ShuffledLegalActions(state))
			{
				searchLimit.IncreaseDepth();
				double expectedValue = ExpectedValue(new GameState(state), action, searchLimit);
				searchLimit.DecreaseDepth();

				if (expectedValue > maxValue)
					maxValue = expectedValue;
			}

			return maxValue;
		}

		private List<Action> ShuffledLegalActions(GameState state)
		{
			var legalActions = new List<Action>(state.GetLegalActions());
			RandomProvider.Shuffle(legalActions);
			return legalActions;
		}

		/*
		 * Returns an estimated value of the given state. The estimate is a score from
		 * 0 to 100 calculated as follows:
		 *		+ up to 60 points proportional to the ratio of empty cells to total cells
		 *		+ up to 20 points proportional to the ratio of the highest number to the goal number
		 *		+ 10 points if the highest valued tile is in a corner of the grid
		 *		+ 10 points if the highest valued tile has an adjacent tile that is half its value
		 */
		private double Evaluate(GameState state)
		{
			double score = 0;
			double emptyCellRatio = state.EmptyCells / (double)state.Grid.Cells;
			score += emptyCellRatio * 60;

			double goalRatio = 2.0 * state.HighestNumber / state.GoalNumber;
			score += 20 * goalRatio;

			var highestValuedTile = state.GetTiles().First(tile => tile.Value == state.HighestNumber);
			if (IsCornerCell(highestValuedTile.Cell))
				score += 10;

			foreach (var neighbor in state.Grid.GetNeighbors(highestValuedTile.Cell, true))
			{
				if (neighbor == state.HighestNumber / 2)
				{
					score += 10;
					break;
				}
			}

			return score;
		}

		private bool IsCornerCell(GridCell cell)
		{
			int gridEdge = GameState.GRID_SIZE - 1;
			bool isEdgeRow = cell.Row == 0 || cell.Row == gridEdge;
			bool isEdgeColumn = cell.Column == 0 || cell.Column == gridEdge;
			return isEdgeRow && isEdgeColumn;
		}
	}
}
