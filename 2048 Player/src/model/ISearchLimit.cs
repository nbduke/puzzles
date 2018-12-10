namespace Player.Model
{
	/// <summary>
	/// Defines dynamic search limits for the expectimax algorithm.
	/// </summary>
	public interface ISearchLimit
	{
		/// <summary>
		/// Returns true when the search limit is reached.
		/// </summary>
		bool Done();

		/// <summary>
		/// Records an increase in the depth of the decision tree.
		/// </summary>
		void IncreaseDepth();

		/// <summary>
		/// Records a decrease in the depth of the decision tree.
		/// </summary>
		void DecreaseDepth();
	}
}
