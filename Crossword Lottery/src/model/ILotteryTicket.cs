using System.Collections.Generic;

namespace CrosswordLottery.Model
{
	public interface ILotteryTicket
	{
		uint NumberOfGivenCharacters
		{
			get;
		}

		double MaxPrize
		{
			get;
		}

		CrosswordContent Crossword
		{
			get;
		}

		double GetPrize(IEnumerable<char> givenCharacters);
	}
}
