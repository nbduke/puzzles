using System;

using Tools;

namespace Player.Model
{
	public delegate void ActionTakenHandler(Action action, GameState resultState);
	public delegate void GameStartedHandler(GameState initialState);
	public delegate void GameEndedHandler(GameStats result);

	/// <summary>
	/// Simulates playing the 2048 game.
	/// </summary>
	public class GameSimulator
	{
		public const double TILE_PROB_2 = 0.9;
		public const double TILE_PROB_4 = 1.0 - TILE_PROB_2;

		private readonly int GamesToPlay;
		private readonly Func<GameState> GetInitialState;
		private readonly IGamePlayer Player;

		/// <summary>
		/// Creates a simulation where each game will start from a new random state.
		/// </summary>
		/// <param name="gamesToPlay">the number of games to play</param>
		/// <param name="player">the player providing action decisions</param>
		/// <param name="goalNumber">the goal number for each new state</param>
		public GameSimulator(
			int gamesToPlay,
			IGamePlayer player = null,
			int goalNumber = GameState.DEFAULT_GOAL)
			: this(gamesToPlay, player)
		{
			GetInitialState = () =>
			{
				return RandomInitialState(goalNumber);
			};
		}

		/// <summary>
		/// Creates a simulation where each game will start from a fixed state.
		/// </summary>
		/// <param name="gamesToPlay">the number of games to play</param>
		/// <param name="initialState">the starting state for each game</param>
		/// <param name="player">the player providing action decisions</param>
		public GameSimulator(
			int gamesToPlay,
			GameState initialState,
			IGamePlayer player = null)
			: this(gamesToPlay, player)
		{
			GetInitialState = () => initialState;
		}

		private GameSimulator(int gamesToPlay, IGamePlayer player)
		{
			Validate.IsTrue(gamesToPlay >= 0, "Cannot simulate a negative number of games");

			GamesToPlay = gamesToPlay;

			if (player != null)
				Player = player;
			else
				Player = new ExpectimaxPlayer(TILE_PROB_2);
		}

		/// <summary>
		/// Creates an initial state with two tiles placed randomly.
		/// </summary>
		/// <param name="goalNumber">the goal number for the state (default is 2048)</param>
		public static GameState RandomInitialState(int goalNumber = GameState.DEFAULT_GOAL)
		{
			GameState state = new GameState(goalNumber);
			state.AddRandomTile(TILE_PROB_2);
			state.AddRandomTile(TILE_PROB_2);
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
				state.AddRandomTile(TILE_PROB_2);

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
		/// Runs the simulation.
		/// </summary>
		/// <param name="shouldStop">an optional parameter allowing the simulation to be
		/// interrupted at any time</param>
		/// <returns>aggregated statistics over all games played</returns>
		public AggregateStats Run(Func<bool> shouldStop = null)
		{
			if (shouldStop == null)
				shouldStop = () => false;

			var stats = new AggregateStats();
			for (int i = 0; i < GamesToPlay && !shouldStop(); ++i)
			{
				var initialState = GetInitialState();
				GameStarted(initialState);

				var gameResult = PlayGame(initialState, shouldStop);
				stats.RecordGame(gameResult);
				GameEnded(gameResult);
			}

			return stats;
		}

		private GameStats PlayGame(GameState initialState, Func<bool> shouldStop)
		{
			GameState state = new GameState(initialState);
			int turnsTaken = 0;
			DateTime start = DateTime.Now;
			Action action = Player.GetPolicy(state);

			while (action != Action.NoAction && !shouldStop())
			{
				SimulateAction(state, action);
				++turnsTaken;
				ActionTaken(action, state);
				action = Player.GetPolicy(state);
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
