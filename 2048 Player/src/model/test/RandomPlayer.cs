using System.Collections.Generic;

using Player.Model;
using Tools.Math;

namespace Test
{
	/*
	 * An IGamePlayer that plays randomly. This can provide a control
	 * for evaluating changes to intelligent agent algorithms.
	 */
	class RandomPlayer : IGamePlayer
	{
		/*
		 * Returns one of the state's legal actions at random, or NoAction if there
		 * are no legal actions.
		 */
		public Action GetPolicy(GameState state)
		{
			var legalActions = new List<Action>(state.GetLegalActions());
			if (legalActions.Count == 0)
				return Action.NoAction;
			else
				return RandomProvider.Select(legalActions);
		}
	}
}