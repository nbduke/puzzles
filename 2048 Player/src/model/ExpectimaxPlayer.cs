using System.Collections.Generic;

using Tools.Math;

namespace Player.Model
{
	/// <summary>
	/// ExpectimaxPlayer is a parallelized expectimax algorithm implementation applied to the
	/// 2048 game. The class exposes methods for determining the optimal action to take for a
	/// particular game state.
	/// </summary>
	public class ExpectimaxPlayer
	{
		public const double TILE_PROB_2 = 0.9;
		public const double TILE_PROB_4 = 1.0 - TILE_PROB_2;

		private const double NO_VALUE = double.MinValue;

		private readonly Action[] Actions = new Action[]
		{
			Action.Left, Action.Up, Action.Right, Action.Down
		};

		/// <summary>
		/// Returns the best action to take in a given state, with its expected value.
		/// </summary>
		/// <param name="state">the state</param>
		/// <param name="searchLimit">defines limits on the search algorithm</param>
		/// <returns></returns>
		public ActionValue GetPolicy(GameState state, ISearchLimit searchLimit)
		{
			ActionValue result = new ActionValue()
			{
				Action = Action.NoAction,
				Value = NO_VALUE
			};

			foreach (var actionValue in GetPolicies(state, searchLimit))
			{
				if (actionValue.Value > result.Value)
					result = actionValue;
			}

			return result;
		}

		/// <summary>
		/// Returns an enumerable of actions that may be taken in a given state with
		/// their corresponding expected values.
		/// </summary>
		/// <remarks>
		/// Higher expected values imply better chances of winning from taking the
		/// corresponding action.
		/// </remarks>
		/// <param name="state">the state</param>
		/// <param name="searchLimit">defines limits on the search algorithm</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state, ISearchLimit searchLimit)
		{
			var legalActions = new List<Action>(state.GetLegalActions());
			RandomProvider.Shuffle(legalActions); // break ties randomly

			foreach (var action in legalActions)
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
		private double ExpectedValue(GameState state, Action action, ISearchLimit searchLimit)
		{
			if (!state.DoAction(action))
				return NO_VALUE; // the action was illegal

			var emptyCells = state.GetEmptyCells();
			double placementProbability = 1.0 / (GameState.TOTAL_CELLS - state.CellsFilled);
			double probability_2 = placementProbability * TILE_PROB_2;
			double probability_4 = placementProbability * TILE_PROB_4;
			double expectedValue = 0;

			// Tries all possible placements of '2' and '4' tiles, calculating the
			// value of each outcome and aggregating values into the expected value.
			foreach (var cell in emptyCells)
			{
				var tile_2 = new Tile(cell, 2);
				state.AddTile(tile_2);
				expectedValue += MaxValue(state, searchLimit) * probability_2;
				state.RemoveTile(cell);

				var tile_4 = new Tile(cell, 4);
				state.AddTile(tile_4);
				expectedValue += MaxValue(state, searchLimit) * probability_4;
				state.RemoveTile(cell);
			}

			return expectedValue;
		}

		/*
		 * Returns the maximum value over all states reachable from the given state.
		 */
		private double MaxValue(GameState state, ISearchLimit searchLimit)
		{
			if (state.IsWin)
				return 100.0 * state.GoalNumber * state.GoalNumber;
			else if (state.IsLoss)
				return 0;
			else if (searchLimit.Done())
				return Evaluate(state);

			double maxValue = NO_VALUE;
			foreach (var action in Actions)
			{
				searchLimit.IncreaseDepth();
				double expectedValue = ExpectedValue(new GameState(state), action, searchLimit);
				searchLimit.DecreaseDepth();

				if (expectedValue > maxValue)
					maxValue = expectedValue;
			}

			return maxValue;
		}

		/*
		 * Returns an estimated value of the given state. The estimate is calculated by
		 * dividing the sum of the squared numbers on the grid by the squared count of
		 * tiles.
		 */
		public double Evaluate(GameState state)
		{
			double sumSq = 0;
			foreach (Tile t in state.GetTiles())
			{
				sumSq += t.Value * t.Value;
			}

			double score = sumSq / (state.CellsFilled * state.CellsFilled);

			if (state[0, 0] == state.HighestNumber ||
				state[0, 3] == state.HighestNumber ||
				state[3, 0] == state.HighestNumber ||
				state[3, 3] == state.HighestNumber)
				score *= 1.5;

			return score;
		}
	}
}
