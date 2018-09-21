using System;
using System.Collections.Generic;
using System.Linq;

namespace CrosswordLottery.Model
{
	public class LotteryTicket : ILotteryTicket
	{
		public uint NumberOfGivenCharacters { get; private set; }
		public CrosswordContent Crossword { get; private set; }

		private SortedDictionary<uint, double> PrizeTable { get; set; }

		public LotteryTicket(
			uint numberOfGivenCharacters,
			CrosswordContent crossword,
			SortedDictionary<uint, double> prizeTable)
		{
			if (numberOfGivenCharacters > (uint)Constants.Alphabet.Length)
				throw new ArgumentException("The number of given characters cannot exceed the alphabet size.");
			if (crossword == null)
				throw new ArgumentNullException("crossword");
			if (prizeTable == null)
				throw new ArgumentNullException("prizeTable");

			NumberOfGivenCharacters = numberOfGivenCharacters;
			Crossword = crossword;
			PrizeTable = prizeTable;
		}

		public double MaxPrize
		{
			get
			{
				return PrizeTable.Values.Last();
			}
		}

		public double GetPrize(IEnumerable<char> givenCharacters)
		{
			uint numWordsRevealed = Crossword.CountRevealedWords(givenCharacters);

			foreach (uint rewardThreshold in PrizeTable.Keys.Reverse())
			{
				if (rewardThreshold <= numWordsRevealed)
					return PrizeTable[rewardThreshold];
			}

			return 0;
		}
	}
}
