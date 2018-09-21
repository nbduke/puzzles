using Tools.DataStructures;
using Tools.Math;

namespace CrosswordLottery.Model
{
	public class M0 : ILotteryModel
	{
		private Arrangement<char> Alphabet = new Arrangement<char>(Constants.Alphabet);

		public string GetName()
		{
			return "Uniform Model";
		}

		public double GetExpectedPrize(ILotteryTicket ticket)
		{
			double expectedPrize = 0;
			double probability = 1.0 / Combinatorics.GetNumberOfCombinations(
				Constants.AlphabetSize, ticket.NumberOfGivenCharacters);

			foreach (var combo in Alphabet.GetCombinations(ticket.NumberOfGivenCharacters))
			{
				double prize = ticket.GetPrize(combo);
				expectedPrize += prize * probability;
			}

			return expectedPrize;
		}
	}
}
