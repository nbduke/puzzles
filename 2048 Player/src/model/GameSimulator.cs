using System;

using Tools;

namespace Player.Model
{
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
		/// Plays a number of 2048 games with a different, random initial state each game.
		/// </summary>
		/// <param name="games">the number of games to play</param>
		/// <param name="goalNumber">the goal number of each game (default is 2048)</param>
		/// <param name="logEachGame">if true, each game's result will be logged</param>
		/// <returns>aggregate statistics over all games</returns>
		public AggregateStats Play(
			int games,
			int goalNumber = GameState.DEFAULT_GOAL,
			bool logEachGame = false)
		{
			Validate.IsTrue(games >= 0, "games cannot be negative");

			if (logEachGame)
				Console.WriteLine($"Playing {games} games to {goalNumber}. Each game starts with a new random state.");

			return PlayGames(games, logEachGame, () =>
			{
				return GameState.RandomInitialState(goalNumber);
			});
		}

		/// <summary>
		/// Plays a number of 2048 games from a fixed starting state.
		/// </summary>
		/// <param name="games">the number of games to play</param>
		/// <param name="initialState">the starting state</param>
		/// <param name="logEachGame">if true, each game's result will be logged</param>
		/// <returns>aggregate statistics over all games</returns>
		public AggregateStats Play(int games, GameState initialState, bool logEachGame = false)
		{
			Validate.IsTrue(games >= 0, "games cannot be negative");

			if (logEachGame)
				Console.WriteLine($"Playing {games} games to {initialState.GoalNumber}. Each game starts with the same state.");

			return PlayGames(games, logEachGame, () => initialState);
		}

		private AggregateStats PlayGames(int games, bool logEachGame, Func<GameState> getInitialState)
		{
			var stats = new AggregateStats();
			for (int i = 0; i < games; ++i)
			{
				var initialState = getInitialState();

				if (logEachGame)
				{
					Console.WriteLine($"\n------------------ Game {i + 1} ------------------");
					Console.WriteLine(initialState.ToString());
					Console.WriteLine("...");
				}

				var gameResult = PlayGame(initialState);
				stats.RecordGame(gameResult);

				if (logEachGame)
				{
					Console.WriteLine(gameResult.FinalState.ToString());
					Console.WriteLine(gameResult.WasWon ? "Win! :)" : "Loss :(");
					Console.WriteLine($"Turns taken: {gameResult.TurnsTaken}");
					Console.WriteLine($"Duration: {Math.Round(gameResult.DurationMinutes)} min");
				}
			}

			return stats;
		}

		private GameStats PlayGame(GameState initialState)
		{
			GameState state = new GameState(initialState);
			int turnsTaken = 0;
			ActionValue av;

			DateTime start = DateTime.Now;
			do
			{
				av = Player.GetPolicy(state, SetDepthLimit(state));
				state.DoAction(av.Action);
				state.AddRandomTile(ExpectimaxPlayer.TILE_PROB_2);
				++turnsTaken;
			} while (av.Action != Action.NoAction);
			DateTime end = DateTime.Now;

			return new GameStats()
			{
				InitialState = initialState,
				FinalState = state,
				TurnsTaken = turnsTaken,
				DurationMinutes = (end - start).TotalMinutes
			};
		}

		private DepthLimit SetDepthLimit(GameState state)
		{
			if (state.CellsFilled <= 4)
				return new DepthLimit(3);
			else if (state.CellsFilled <= 8)
				return new DepthLimit(4);
			else if (state.CellsFilled <= 12)
				return new DepthLimit(6);
			else
				return new DepthLimit(8);
		}
	}
}
