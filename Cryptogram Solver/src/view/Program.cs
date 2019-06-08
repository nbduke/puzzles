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
			Parser.Default.ParseArguments<Options>(args).WithParsed(HandleInput);
		}

		private static void HandleInput(Options options)
		{
			if (string.IsNullOrEmpty(options.Cryptogram) &&
				string.IsNullOrEmpty(options.CryptogramFile))
			{
				Console.Error.WriteLine(
					"You must provide either a cryptogram instance or " +
					"the path to a file containing a cryptogram instance."
				);
				return;
			}

			string cryptogram = options.Cryptogram;
			if (string.IsNullOrEmpty(cryptogram))
			{
				cryptogram = ReadCryptogram(options.CryptogramFile);
			}

			SolvePuzzle(cryptogram, ReadDictionary(options.Dictionary));
		}

		private static void SolvePuzzle(string cryptogram, IEnumerable<string> dictionary)
		{
			Console.WriteLine(cryptogram);
			Console.WriteLine("Solving...");

			string solution = Model.CryptogramSolver.Solve(cryptogram, dictionary);

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

		private static string ReadCryptogram(string cryptogramFile)
		{
			using (var reader = new StreamReader(cryptogramFile))
			{
				return reader.ReadLine().Trim();
			}
		}
	}
}
