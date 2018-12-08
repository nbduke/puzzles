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
		public double MinGameDurationMinutes { get; private set; } = double.MaxValue;
		public double MaxGameDurationMinutes { get; private set; } = 0;
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

		public double AverageGameDurationMinutes
		{
			get { return TotalDurationMinutes / GamesPlayed; }
		}

		public double AverageTurnDurationSeconds
		{
			get { return TotalDurationMinutes * 60 / TotalTurnsTaken; }
		}

		private readonly Counter<int> NumbersReached = new Counter<int>();

		/// <summary>
		/// Updates statistics for a single game.
		/// </summary>
		/// <param name="game">the game</param>
		public void RecordGame(GameStats game)
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

			if (game.DurationMinutes < MinGameDurationMinutes)
				MinGameDurationMinutes = game.DurationMinutes;
			if (game.DurationMinutes > MaxGameDurationMinutes)
				MaxGameDurationMinutes = game.DurationMinutes;

			NumbersReached.Increment(game.HighestNumberReached);
			if (NumbersReached[game.HighestNumberReached] > NumbersReached[MostCommonNumberReached])
				MostCommonNumberReached = game.HighestNumberReached;
		}
	}

	/// <summary>
	/// Represents statistics on a single game of 2048, including the initial and
	/// final states, the duration in minutes, etc.
	/// </summary>
	public struct GameStats
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