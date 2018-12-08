using System;

using Tools;

namespace Player.Model
{
	public delegate void ActionTakenHandler(Action action, GameState resultState);
	public delegate void GameStartedHandler(GameState initialState);
	public delegate void GameEndedHandler(GameStats result);

	/// <summary>
	/// Simulates playing the 2048 game using ExpectimaxLearner to choose the action at each step.
	/// </summary>
	public class GameSimulator
	{
		private readonly ExpectimaxPlayer Player;

		public GameSimulator()
		{
			Player = new ExpectimaxPlayer();
		}

		/// <summary>
		/// Creates an initial state with two tiles placed randomly.
		/// </summary>
		/// <param name="goalNumber">the goal number for the state (default is 2048)</param>
		public static GameState RandomInitialState(int goalNumber = GameState.DEFAULT_GOAL)
		{
			GameState state = new GameState(goalNumber);
			state.AddRandomTile(ExpectimaxPlayer.TILE_PROB_2);
			state.AddRandomTile(ExpectimaxPlayer.TILE_PROB_2);
			return state;
		}

		/// <summary>
		/// Applies an action to a game state and performs the nondeterministic
		/// portion of the state update.
		/// </summary>
		/// <param name="state">the game state</param>
		/// <param name="action">the action</param>
		/// <returns>true if the action is legal</returns>
		public static bool SimulateAction(GameState state, Action action)
		{
			bool wasLegal = state.DoAction(action);
			if (wasLegal)
				state.AddRandomTile(ExpectimaxPlayer.TILE_PROB_2);

			return wasLegal;
		}

		/// <summary>
		/// Invoked when an action is taken by the simulator.
		/// </summary>
		public event ActionTakenHandler ActionTaken = delegate { };

		/// <summary>
		/// Invoked when a new game starts.
		/// </summary>
		public event GameStartedHandler GameStarted = delegate { };

		/// <summary>
		/// Invoked when a game ends.
		/// </summary>
		public event GameEndedHandler GameEnded = delegate { };

		/// <summary>
		/// Plays a number of 2048 games with a different, random initial state each game.
		/// </summary>
		/// <param name="games">the number of games to play</param>
		/// <param name="goalNumber">the goal number of each game (default is 2048)</param>
		/// <param name="shouldStop">allows the simulation to be interrupted at any time</returns>
		public AggregateStats Play(
			int games,
			int goalNumber = GameState.DEFAULT_GOAL,
			Func<bool> shouldStop = null)
		{
			Validate.IsTrue(games >= 0, "games cannot be negative");

			if (shouldStop == null)
				shouldStop = () => false;

			return PlayGames(games, shouldStop, () =>
			{
				return RandomInitialState(goalNumber);
			});
		}

		/// <summary>
		/// Plays a number of 2048 games from a fixed starting state.
		/// </summary>
		/// <param name="games">the number of games to play</param>
		/// <param name="initialState">the starting state</param>
		/// <param name="shouldStop">allows the simulation to be interrupted at any time</returns>
		public AggregateStats Play(int games, GameState initialState, Func<bool> shouldStop = null)
		{
			Validate.IsTrue(games >= 0, "games cannot be negative");

			if (shouldStop == null)
				shouldStop = () => false;

			return PlayGames(games, shouldStop, () => initialState);
		}

		private AggregateStats PlayGames(int games, Func<bool> shouldStop, Func<GameState> getInitialState)
		{
			var stats = new AggregateStats();
			for (int i = 0; i < games && !shouldStop(); ++i)
			{
				var initialState = getInitialState();
				GameStarted(initialState);

				var gameResult = PlayGame(initialState, shouldStop);
				GameEnded(gameResult);
				stats.RecordGame(gameResult);
			}

			return stats;
		}

		private GameStats PlayGame(GameState initialState, Func<bool> shouldStop)
		{
			GameState state = new GameState(initialState);
			int turnsTaken = 0;
			DateTime start = DateTime.Now;
			ActionValue av = Player.GetPolicy(state, new DepthLimit(state));

			while (av.Action != Action.NoAction && !shouldStop())
			{
				SimulateAction(state, av.Action);
				++turnsTaken;
				ActionTaken(av.Action, state);
				av = Player.GetPolicy(state, new DepthLimit(state));
			}
			DateTime end = DateTime.Now;

			return new GameStats()
			{
				InitialState = initialState,
				FinalState = state,
				TurnsTaken = turnsTaken,
				DurationMinutes = (end - start).TotalMinutes
			};
		}
	}
}
