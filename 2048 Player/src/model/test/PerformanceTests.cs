using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Player.Model;

namespace Test
{
	[TestClass]
	public class PerformanceTests
	{
		private const int GAMES_PER_TRIAL = 10;

		[TestMethod]
		public void RandomPlayer()
		{
			var simulator = new GameSimulator(GAMES_PER_TRIAL, new RandomPlayer());
			RunSimulation(simulator);
		}

		[TestMethod]
		public void ExpectimaxPlayer()
		{
			var simulator = new GameSimulator(GAMES_PER_TRIAL, new ExpectimaxPlayer());
			RunSimulation(simulator);
		}

		[TestMethod]
		public void MonteCarloPlayer()
		{
			var simulator = new GameSimulator(GAMES_PER_TRIAL, new MonteCarloPlayer());
			RunSimulation(simulator);
		}

		private void RunSimulation(GameSimulator simulator)
		{
			simulator.GameStarted += state =>
			{
				Console.WriteLine("Started game");
				Console.Write(state);
			};
			simulator.GameEnded += stats =>
			{
				if (stats.WasWon)
					Console.WriteLine("Game won");
				else
					Console.WriteLine("Game lost");

				Console.WriteLine(stats.FinalState);
				Console.WriteLine();
			};

			var finalStats = simulator.Run();
			Console.WriteLine($"Games played\t\t\t: {finalStats.GamesPlayed}");
			Console.WriteLine($"Games won\t\t\t: {finalStats.Wins}");
			Console.WriteLine($"Total duration\t\t\t: {finalStats.TotalDurationMinutes} min");
			Console.WriteLine($"Average turns taken\t\t: {finalStats.AverageTurnsTaken}");
			Console.WriteLine($"Average game duration\t\t: {finalStats.AverageGameDurationMinutes} min");
			Console.WriteLine($"Average turn duration\t\t: {finalStats.AverageTurnDurationSeconds} sec");
			Console.WriteLine($"Most common number reached\t: {finalStats.MostCommonNumberReached}");
		}
	}
}