using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Math;

namespace Player.Model
{
	/// <summary>
	/// Applies Monte Carlo Tree Search to play the 2048 game.
	/// </summary>
	public class MonteCarloPlayer : IGamePlayer {
		private const double EXPLORATION_RATE = 2;
		private DecisionNode TreeRoot;

		/// <summary>
		/// Returns the best action to take in the given state using a smart
		/// time limit for sampling the search space.
		/// </summary>
		/// <param name="state">the game state</param>
		public Action GetPolicy(GameState state)
		{
			return GetPolicy(state, new TimeLimit(state));
		}

		/// <summary>
		/// Returns the best action to take in the given state using the given
		/// search limit.
		/// </summary>
		/// <param name="state">the game state</param>
		/// <param name="searchLimit">the search limit</param>
		/// <returns></returns>
		public Action GetPolicy(GameState state, ISearchLimit searchLimit)
		{
			return GetPolicies(state, searchLimit).Best();
		}

		/// <summary>
		/// Returns the set of legal actions in the given state with their corresponding
		/// expected values. Uses a smart time limit.
		/// </summary>
		/// <param name="state">the game state</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state)
		{
			return GetPolicies(state, new TimeLimit(state));
		}

		/// <summary>
		/// Returns the set of legal actions in the given state with their corresponding
		/// expected values.
		/// </summary>
		/// <param name="state">the game state</param>
		/// <param name="searchLimit">a search limit for the algorithm</param>
		public IEnumerable<ActionValue> GetPolicies(GameState state, ISearchLimit searchLimit)
		{
			TreeRoot = new DecisionNode(state);
			while (!searchLimit.Done())
			{
				SampleSearchTree(TreeRoot);
			}

			var results = new List<ActionValue>(TreeRoot.Children.Select(pair => new ActionValue()
			{
				Action = pair.Key,
				Value = pair.Value.VisitCount > 0 ? pair.Value.AverageValue : 0
			}));
			RandomProvider.Shuffle(results); // break ties randomly

			return results;
		}

		private double SampleSearchTree(BaseNode currentNode)
		{
			double value = currentNode.IsDecisionNode
				? GetDecisionNodeValue((DecisionNode)currentNode)
				: GetChanceNodeValue((ChanceNode)currentNode);
			currentNode.TotalValue += value;
			++currentNode.VisitCount;

			return value;
		}

		private double GetDecisionNodeValue(DecisionNode node)
		{
			if (node != TreeRoot && node.VisitCount == 0)
			{
				return DoRollout(node);
			}
			else
			{
				node.ExpandChildren();
				if (node.Children.Count == 0)
				{
					return GetValueFor(node.State);
				}
				else
				{
					var child = SelectChildToVisit(node);
					return SampleSearchTree(child);
				}
			}
		}

		private double GetChanceNodeValue(ChanceNode node)
		{
			var child = node.GenerateChild();
			return SampleSearchTree(child);
		}

		/*
		 * Uses the UCB for Trees algorithm to select children for exploration.
		 * If any children are unexplored, a child is selected from them uniformly
		 * at random. Otherwise, the child with the maximal UCB is chosen.
		 */
		private ChanceNode SelectChildToVisit(DecisionNode node)
		{
			double maxUCB = double.MinValue;
			ChanceNode bestChild = null;
			var unvisitedChildren = new List<ChanceNode>();

			foreach (var child in node.Children.Values)
			{
				if (child.VisitCount == 0)
				{
					unvisitedChildren.Add(child);
				}
				else
				{
					double ucb = CalculateUCB(node, child);
					if (ucb > maxUCB)
					{
						maxUCB = ucb;
						bestChild = child;
					}
				}
			}

			if (unvisitedChildren.Count == 0)
				return bestChild;
			else
				return RandomProvider.Select(unvisitedChildren);
		}

		private double CalculateUCB(DecisionNode parent, ChanceNode child)
		{
			double v = child.AverageValue;
			double n = Math.Log(parent.VisitCount) / child.VisitCount;
			return v + EXPLORATION_RATE * Math.Sqrt(n);
		}

		/*
		 * Plays randomly until a terminal state is reached.
		 */
		private double DoRollout(DecisionNode node)
		{
			var currentState = new GameState(node.State);
			var legalActions = new List<Action>(currentState.GetLegalActions());

			while (legalActions.Count > 0)
			{
				Action randomAction = RandomProvider.Select(legalActions);
				currentState.ApplyAction(randomAction);
				currentState.AddRandomTile();
				legalActions = new List<Action>(currentState.GetLegalActions());
			}

			return GetValueFor(currentState);
		}

		private double GetValueFor(GameState state)
		{
			double progressTowardGoal = state.HighestNumber / (double)state.GoalNumber;
			double emptyCellsRatio = state.EmptyCells / (double)(GameState.GRID_SIZE * GameState.GRID_SIZE);
			return (progressTowardGoal + emptyCellsRatio) / 2;
		}
	}
}