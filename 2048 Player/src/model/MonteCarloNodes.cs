using System.Collections.Generic;
using Tools.DataStructures;

namespace Player.Model
{
	class BaseNode
	{
		public readonly GameState State;
		public readonly bool IsDecisionNode;
		public double TotalValue;
		public int VisitCount;

		public double AverageValue
		{
			get { return TotalValue / VisitCount; }
		}

		public BaseNode(GameState state, bool isDecisionNode)
		{
			State = state;
			IsDecisionNode = isDecisionNode;
			TotalValue = 0;
			VisitCount = 0;
		}
	}

	/*
	 * Represents nodes in the Monte Carlo Tree Search where a deterministic
	 * decision is made.
	 */
	class DecisionNode : BaseNode
	{
		public readonly Dictionary<Action, ChanceNode> Children = new Dictionary<Action, ChanceNode>();
		private bool IsExpanded = false;

		public DecisionNode(GameState state): base(state, true)
		{
		}

		public void ExpandChildren()
		{
			if (!IsExpanded)
			{
				foreach (Action action in State.GetLegalActions())
				{
					var nextState = new GameState(State);
					nextState.ApplyAction(action);
					Children.Add(action, new ChanceNode(nextState));
				}
				IsExpanded = true;
			}
		}
	}

	/*
	 * Represents nodes in the Monte Carlo Tree Search where stochastic events occur.
	 */
	class ChanceNode : BaseNode
	{
		public readonly Dictionary<GridCell, DecisionNode> Children = new Dictionary<GridCell, DecisionNode>();

		public ChanceNode(GameState state): base(state, false)
		{
		}

		public DecisionNode GenerateChild()
		{
			var nextState = new GameState(State);
			var addedTile = nextState.AddRandomTile();

			if (!Children.TryGetValue(addedTile.Cell, out DecisionNode child))
			{
				child = new DecisionNode(nextState);
				Children.Add(addedTile.Cell, child);
			}

			return child;
		}
	}
}