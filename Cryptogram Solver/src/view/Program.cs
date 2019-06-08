using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;

namespace CryptogramSolver.View
{
	class Program
	{
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed(SolvePuzzle);
		}

		private static void SolvePuzzle(Options options)
		{
			Console.WriteLine(options.Cryptogram);
			Console.WriteLine("Solving...");

			string solution = Model.CryptogramSolver.Solve(
				options.Cryptogram,
				ReadDictionary(options.Dictionary)
			);

			if (string.IsNullOrEmpty(solution))
				Console.WriteLine("We could not find a solution :(");
			else
				Console.WriteLine(solution);
		}

		private static IEnumerable<string> ReadDictionary(string dictionaryFile)
		{
			using (var reader = new StreamReader(dictionaryFile))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine().Trim();
					if (line.Length > 0)
						yield return line;
				}
			}
		}
	}
}
