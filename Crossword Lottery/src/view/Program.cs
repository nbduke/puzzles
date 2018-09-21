using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using CrosswordLottery.Model;

namespace CrosswordLottery.View
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Help();
				return;
			}

			// Parse command-line arguments
			string inputFileName = args[0];
			string option = args[1];

			if (args[1].ToUpper() != "-L") // TODO eventually implement -G to parse grid
			{
				Console.WriteLine("Unrecognized option: {0}", args[1]);
				Help();
				return;
			}

			uint numGivenCharacters = 0;
			List<string> wordList = null;
			SortedDictionary<uint, double> prizeTable = null;

			// Parse the input file into a LotteryTicket
			try
			{
				StreamReader inputFile = new StreamReader(inputFileName);

				string firstLine = inputFile.ReadLine();
				numGivenCharacters = uint.Parse(firstLine);

				wordList = ParseWordList(inputFile);
				prizeTable = ParsePrizeTable(inputFile);
			}
			catch(Exception)
			{
				Console.WriteLine("Error parsing input file.");
				return;
			}

			CrosswordContent crossword = new CrosswordContent { WordList = wordList };
			LotteryTicket ticket = new LotteryTicket(numGivenCharacters, crossword, prizeTable);

			// Construct models and run them in parallel
			ILotteryModel[] models = {
										   new M0(),
										   new M1(),
										   new M2(4 /*numGivenVowels*/)
									   };

			Parallel.ForEach(models,
				(model) =>
				{
					double expectedPrize = model.GetExpectedPrize(ticket);
					lock (Console.Out)
					{
						Console.WriteLine("Expected value from {0}: {1}", model.GetName(), expectedPrize);
					}
				});
		}

		private static void Help()
		{
			Console.WriteLine("CrosswordLotteryApp <input_file> <-L | -G>");
		}

		private static List<string> ParseWordList(StreamReader inputFile)
		{
			List<string> wordList = new List<string>();
			while (!inputFile.EndOfStream)
			{
				string word = inputFile.ReadLine();

				if (string.IsNullOrEmpty(word))
					break;

				wordList.Add(word.ToLower());
			}

			return wordList;
		}

		private static SortedDictionary<uint, double> ParsePrizeTable(StreamReader inputFile)
		{
			SortedDictionary<uint, double> prizeTable = new SortedDictionary<uint, double>();
			while (!inputFile.EndOfStream)
			{
				string line = inputFile.ReadLine();
				var parts = line.Split(',');

				if (parts.Length != 2)
					throw new InvalidDataException("Unexpected input file format.");

				uint prizeThreshold = uint.Parse(parts[0]);
				double prize = double.Parse(parts[1]);
				prizeTable.Add(prizeThreshold, prize);
			}

			return prizeTable;
		}
	}
}
