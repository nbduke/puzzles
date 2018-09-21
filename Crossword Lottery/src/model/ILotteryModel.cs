namespace CrosswordLottery.Model
{
	/// <summary>
	/// Represents a model of a crossword lottery ticket's random variable.
	/// </summary>
	public interface ILotteryModel
	{
		string GetName();

		double GetExpectedPrize(ILotteryTicket ticket);
	}
}
