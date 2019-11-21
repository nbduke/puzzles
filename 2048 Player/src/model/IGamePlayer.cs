using System.Collections.Generic;

namespace Player.Model
{
	public interface IGamePlayer
	{
		/// <summary>
		/// Returns the best action to take in a given game state.
		/// </summary>
		/// <param name="state">the game state</param>
		Action GetPolicy(GameState state);

		/// <summary>
		/// Returns the list of legal actions in a game state with their
		/// corresponding expected values.
		/// </summary>
		/// <param name="state">the game state</param>
		IEnumerable<ActionValue> GetPolicies(GameState state);
	}
}