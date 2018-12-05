using System.Collections.Generic;

using Tools;
using Tools.DataStructures;

namespace Player.Model
{
	/// <summary>
	/// Represents statistics aggregated over multiple games. Statistics may be
	/// calculated all at once or updated over time.
	/// </summary>
	public class AggregateStats
	{
		public int GamesPlayed { get; private set; } = 0;
		public int Wins { get; private set; } = 0;
		public int TotalTurnsTaken { get; private set; } = 0;
		public int MinTurnsTaken { get; private set; } = int.MaxValue;
		public int MaxTurnsTaken { get; private set; } = 0;
		public double TotalDurationMinutes { get; private set; } = 0;
		public double MinDurationMinutes { get; private set; } = double.MaxValue;
		public double MaxDurationMinutes { get; private set; } = 0;
		public int MostCommonNumberReached { get; private set; } = 0;

		public int Losses
		{
			get { return GamesPlayed - Wins; }
		}

		public double WinRatio
		{
			get { return Wins / (double)GamesPlayed; }
		}

		public double AverageTurnsTaken
		{
			get { return TotalTurnsTaken / (double)GamesPlayed; }
		}

		public double AverageDurationMinutes
		{
			get { return TotalDurationMinutes / GamesPlayed; }
		}

		private readonly Counter<int> NumbersReached = new Counter<int>();

		/// <summary>
		/// Creates an empty stats object.
		/// </summary>
		/// <remarks>
		/// The initial values should not be accessed as it may cause undefined behavior.
		/// </remarks>
		internal AggregateStats()
		{
		}

		/// <summary>
		/// Calculates stats for a list of games.
		/// </summary>
		/// <param name="games">the list of games</param>
		internal AggregateStats(List<GameStats> games)
		{
			Validate.IsNotNull(games, "games");

			foreach (var game in games)
			{
				RecordGame(game);
			}
		}

		/// <summary>
		/// Updates statistics for a single game.
		/// </summary>
		/// <param name="game">the game</param>
		internal void RecordGame(GameStats game)
		{
			++GamesPlayed;
			if (game.WasWon)
				++Wins;

			TotalTurnsTaken += game.TurnsTaken;
			TotalDurationMinutes += game.DurationMinutes;

			if (game.TurnsTaken < MinTurnsTaken)
				MinTurnsTaken = game.TurnsTaken;
			if (game.TurnsTaken > MaxTurnsTaken)
				MaxTurnsTaken = game.TurnsTaken;

			if (game.DurationMinutes < MinDurationMinutes)
				MinDurationMinutes = game.DurationMinutes;
			if (game.DurationMinutes > MaxDurationMinutes)
				MaxDurationMinutes = game.DurationMinutes;

			NumbersReached.Increment(game.HighestNumberReached);
			if (NumbersReached[game.HighestNumberReached] > NumbersReached[MostCommonNumberReached])
				MostCommonNumberReached = game.HighestNumberReached;
		}
	}

	/// <summary>
	/// Represents statistics on a single game of 2048, including the initial and
	/// final states, the duration in minutes, etc.
	/// </summary>
	struct GameStats
	{
		public GameState InitialState;
		public GameState FinalState;
		public double DurationMinutes;
		public int TurnsTaken;

		public bool WasWon
		{
			get { return FinalState.IsWin; }
		}

		public int HighestNumberReached
		{
			get { return FinalState.HighestNumber; }
		}
	}
}