using System.Collections.Generic;
using System.Linq;

using Tools.DataStructures;

namespace CrosswordLottery.Model
{
	/// <summary>
	/// Builds on the simple adversary model by requiring exactly N vowels
	/// to be given.
	/// </summary>
	public class M2 : ILotteryModel
	{
		private Arrangement<char> Vowels = new Arrangement<char>(Constants.Vowels);
		private Arrangement<char> Consonants = new Arrangement<char>(Constants.Consonants);
		private string Name { get; set; }

		public uint NumberOfGivenVowels { get; private set; }

		public M2(uint numGivenVowels)
		{
			Tools.Validate.IsTrue(numGivenVowels <= Constants.NumberOfVowels,
				"The number of given vowels cannot exceed the number of vowels in the alphabet.");

			NumberOfGivenVowels = numGivenVowels;
			Name = string.Format("{0}-Vowel Adversary Model", NumberOfGivenVowels);
		}

		public string GetName()
		{
			return Name;
		}

		public double GetExpectedPrize(ILotteryTicket ticket)
		{
			if (ticket.NumberOfGivenCharacters == 0)
				return 0;
			else if (ticket.NumberOfGivenCharacters == Constants.AlphabetSize)
				return ticket.MaxPrize;
			else if (NumberOfGivenVowels == Constants.NumberOfVowels)
				return GetExpectedPrizeIncludingAllVowels(ticket);
			else if (ticket.NumberOfGivenCharacters - NumberOfGivenVowels == Constants.NumberOfConsonants)
				return GetExpectedPrizeIncludingAllConsonants(ticket);
			else
				return GetExpectedPrizeExcludingSomeVowelsAndConsonants(ticket);
		}

		private double GetExpectedPrizeIncludingAllVowels(ILotteryTicket ticket)
		{
			uint numExcludedConsonants = Constants.NumberOfConsonants - (ticket.NumberOfGivenCharacters - Constants.NumberOfVowels);
			List<double> prizes = new List<double>();
			List<double> excludedWords = new List<double>();
			double totalExcludedWords = 0;

			foreach (var combo in Consonants.GetCombinations(numExcludedConsonants))
			{
				var givenConsonants = Constants.Consonants.Except(combo);
				var allGivenChars = Constants.Vowels.Concat(givenConsonants);

				double prize = ticket.GetPrize(allGivenChars);
				uint numExcludedWords = ticket.Crossword.CountExcludedWords(combo);

				prizes.Add(prize);
				excludedWords.Add(numExcludedWords);
				totalExcludedWords += numExcludedWords;
			}

			return CalculateExpectedPrize(prizes, excludedWords, totalExcludedWords);
		}

		private double GetExpectedPrizeIncludingAllConsonants(ILotteryTicket ticket)
		{
			uint numExcludedVowels = Constants.NumberOfVowels - NumberOfGivenVowels;
			List<double> prizes = new List<double>();
			List<double> excludedWords = new List<double>();
			double totalExcludedWords = 0;

			foreach (var combo in Vowels.GetCombinations(numExcludedVowels))
			{
				var givenVowels = Constants.Vowels.Except(combo);
				var allGivenChars = Constants.Consonants.Concat(givenVowels);

				double prize = ticket.GetPrize(allGivenChars);
				uint numExcludedWords = ticket.Crossword.CountExcludedWords(combo);

				prizes.Add(prize);
				excludedWords.Add(numExcludedWords);
				totalExcludedWords += numExcludedWords;
			}

			return CalculateExpectedPrize(prizes, excludedWords, totalExcludedWords);
		}

		private double GetExpectedPrizeExcludingSomeVowelsAndConsonants(ILotteryTicket ticket)
		{
			uint numExcludedConsonants = Constants.NumberOfConsonants - (ticket.NumberOfGivenCharacters - NumberOfGivenVowels);
			uint numExcludedVowels = Constants.NumberOfVowels - NumberOfGivenVowels;
			List<double> prizes = new List<double>();
			List<double> excludedWords = new List<double>();
			double totalExcludedWords = 0;

			foreach (var vCombo in Vowels.GetCombinations(numExcludedVowels))
			{
				var givenVowels = Constants.Vowels.Except(vCombo);

				foreach (var cCombo in Consonants.GetCombinations(numExcludedConsonants))
				{
					var givenConsonants = Constants.Consonants.Except(cCombo);
					var allGivenChars = givenVowels.Concat(givenConsonants);
					var allExcludedChars = vCombo.Concat(cCombo);

					double prize = ticket.GetPrize(allGivenChars);
					uint numExcludedWords = ticket.Crossword.CountExcludedWords(allExcludedChars);

					prizes.Add(prize);
					excludedWords.Add(numExcludedWords);
					totalExcludedWords += numExcludedWords;
				}
			}

			return CalculateExpectedPrize(prizes, excludedWords, totalExcludedWords);
		}

		private double CalculateExpectedPrize(List<double> prizes, List<double> excludedWords, double totalExcludedWords)
		{
			double expectedPrize = 0;
			for (int i = 0; i < excludedWords.Count; ++i)
			{
				expectedPrize += prizes[i] * (excludedWords[i] / totalExcludedWords);
			}

			return expectedPrize;
		}
	}
}
