using System.Collections.Generic;

namespace CrosswordLottery.Model
{
	/// <summary>
	/// Represents all of the information on a crossword lottery ticket.
	/// </summary>
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
