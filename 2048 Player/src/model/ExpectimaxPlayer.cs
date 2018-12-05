using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Tools.Math;
using Tools.DataStructures;

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

		private int AvailableTasks;
		private readonly object Mutex;
		private readonly Action[] AllActions;

		public ExpectimaxPlayer()
		{
			AvailableTasks = Math.Max(Environment.ProcessorCount - 1, 0);
			Mutex = new object();
			AllActions = new Action[]
			{
				Action.Up, Action.Down,
				Action.Right, Action.Left
			};
		}

		/// <summary>
		/// Returns the optimal ActionValue for a given state.
		/// </summary>
		/// <remarks>
		/// In expectimax, the optimal action is the one which maximizes the expected
		/// value of the game state that would result from taking that action.
		/// </remarks>
		/// <param name="state">the state</param>
		public ActionValue GetPolicy(GameState state)
		{
			return GetPolicy(state, new NoLimit());
		}

		/// <summary>
		/// Returns the optimal ActionValue for a given state, subject to search limitations.
		/// </summary>
		/// <param name="state">the state</param>
		/// <param name="searchLimit">the limit imposed on the search space</param>
		public ActionValue GetPolicy(GameState state, ISearchLimit searchLimit)
		{
			var allPolicies = GetPolicies(state, searchLimit);
			if (allPolicies.Count() == 0)
				return new ActionValue(Action.NoAction, NO_VALUE);
			else
				return allPolicies.Max((a, b) => a.Value < b.Value);
		}

		/// <summary>
		/// Returns each legal action from the given state with its expected value.
		/// </summary>
		/// <param name="state">the state</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state)
		{
			return GetPolicies(state, new NoLimit());
		}

		/// <summary>
		/// Returns each legal action from the given state with its expected value, subject
		/// to search limitations.
		/// </summary>
		/// <param name="state">the state</param>
		/// <param name="searchLimit">the limit imposed on the search space</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state, ISearchLimit searchLimit)
		{
			RandomProvider.Shuffle(AllActions); // break ties randomly
			searchLimit.NextSearch();

			foreach (var action in AllActions)
			{
				double value = ExpectedValue(new GameState(state), action, searchLimit.Copy());
				if (value != NO_VALUE) // exclude illegal actions
					yield return new ActionValue(action, value);
			}
		}

		/*
		 * Returns the maximum of the expected values of all states that may be reached from the given state.
		 */
		private double MaxValue(GameState state, ISearchLimit searchLimit)
		{
			// Base case: the search limit is reached.
			if (searchLimit.Done())
				return Evaluate(state);

			searchLimit.NextSearch();

			// Determine how many tasks to spawn.
			int tasksToSpawn = 0;
			lock (Mutex)
			{
				tasksToSpawn = Math.Min(AvailableTasks, AllActions.Length - 1); // save one calculation for this thread
				AvailableTasks -= tasksToSpawn;
			}

			// Launch asynchronous tasks to make some of the recursive calls.
			var tasks = new Task<double>[AllActions.Length];
			for (int i = 0; i < tasksToSpawn; ++i)
			{
				var action = AllActions[i];
				tasks[i] = Task.Run(() =>
				{
					return ExpectedValueParallel(new GameState(state), action, searchLimit.Copy());
				});
			}

			// Make any remaining recursive calls in this thread.
			for (int i = tasksToSpawn; i < AllActions.Length; ++i)
			{
				double value = ExpectedValue(new GameState(state), AllActions[i], searchLimit.Copy());
				tasks[i] = Task.FromResult(value);
			}

			// Collect results from the tasks
			Task.WhenAll(tasks).Wait();
			double maxValue = NO_VALUE;

			foreach (var task in tasks)
			{
				if (task.Result > maxValue)
					maxValue = task.Result;
			}

			// If maxValue is equal to NO_VALUE, then no legal actions were found, so
			// return an estimate of the current state's value.
			if (maxValue == NO_VALUE)
				return Evaluate(state);
			else
				return maxValue;
		}

		/*
		 * This method should be called within an asynchronous task. It calls ExpectedValue() and then
		 * updates the available task count.
		 */
		private double ExpectedValueParallel(GameState state, Action action, ISearchLimit searchLimit)
		{
			double value = ExpectedValue(state, action, searchLimit);
			lock (Mutex)
			{
				++AvailableTasks;
			}
			return value;
		}

		/*
		 * Tries every possible nondeterministic outcome of the given state after applying the given action.
		 * The expected value over all outcomes is returned.
		 */
		private double ExpectedValue(GameState state, Action action, ISearchLimit searchLimit)
		{
			// Apply the action. If the action is illegal, return NO_VALUE.
			if (!state.DoAction(action))
				return NO_VALUE;

			var emptyCells = state.GetEmptyCells();
			double tilePlacementProb = 1.0 / emptyCells.Count();
			double expValue = 0;

			// Try all possible '2' tiles
			foreach (var cell in emptyCells)
			{
				var tile = new Tile(cell, 2);
				state.AddTile(tile);
				expValue += tilePlacementProb * TILE_PROB_2 * MaxValue(state, searchLimit);
				state.RemoveTile(tile.Cell);
			}

			// Try all possible '4' tiles
			foreach (var cell in emptyCells)
			{
				var tile = new Tile(cell, 4);
				state.AddTile(tile);
				expValue += tilePlacementProb * TILE_PROB_4 * MaxValue(state, searchLimit);
				state.RemoveTile(tile.Cell); // undo the change
			}

			return expValue;
		}

		/*
		 * Returns an estimated value of the given state. Losing states have value 0. Winning states have a value
		 * equal to the sum of the squared numbers on the grid. All other states' values are equal to the sum of
		 * the squared numbers on the grid divided by the squared count of tiles.
		 */
		public double Evaluate(GameState state)
		{
			if (state.IsLoss)
				return 0;

			// Get the sum of squared tile numbers
			var tiles = state.GetTiles();
			double sumSq = 0;

			foreach (Tile t in tiles)
			{
				sumSq += t.Value * t.Value;
			}

			if (state.IsWin)
				return sumSq;
			else
				return sumSq / (state.CellsFilled * state.CellsFilled);
		}
	}
}
