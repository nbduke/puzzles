namespace CrosswordLottery.Model
{
	public interface ILotteryModel
	{
		string GetName();

		double GetExpectedPrize(ILotteryTicket ticket);
	}
}
