namespace Player.Model
{
	public interface IGamePlayer
	{
		/// <summary>
		/// Returns the best action to take in a given game state.
		/// </summary>
		/// <param name="state">the game state</param>
		Action GetPolicy(GameState state);
	}
}