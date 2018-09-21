using System.Collections.Generic;
using System.Linq;

using Tools.DataStructures;

namespace CrosswordLottery.Model
{
	public class M1 : ILotteryModel
	{
		private Arrangement<char> Alphabet = new Arrangement<char>(Constants.Alphabet);

		public string GetName()
		{
			return "Simple Adversary Model";
		}

		public double GetExpectedPrize(ILotteryTicket ticket)
		{
			if (ticket.NumberOfGivenCharacters == 0)
				return 0;
			else if (ticket.NumberOfGivenCharacters == Constants.AlphabetSize)
				return ticket.MaxPrize;

			uint numExcludedCharacters = Constants.AlphabetSize - ticket.NumberOfGivenCharacters;
			List<double> prizes = new List<double>();
			List<double> excludedWords = new List<double>();
			double totalExcludedWords = 0;

			// For each combination, compute the number of words excluded by the combination and
			// store its prize.
			foreach (var combo in Alphabet.GetCombinations(numExcludedCharacters))
			{
				var givenCharacters = Constants.Alphabet.Except(combo);
				double prize = ticket.GetPrize(givenCharacters);
				uint numExcludedWords = ticket.Crossword.CountExcludedWords(combo);

				prizes.Add(prize);
				excludedWords.Add(numExcludedWords);
				totalExcludedWords += numExcludedWords;
			}

			// Compute the expected prize
			double expectedPrize = 0;
			for (int i = 0; i < excludedWords.Count; ++i)
			{
				expectedPrize += prizes[i] * (excludedWords[i] / totalExcludedWords);
			}

			return expectedPrize;
		}
	}
}
