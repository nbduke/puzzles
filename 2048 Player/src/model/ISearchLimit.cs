namespace Player.Model
{
	/// <summary>
	/// Defines dynamic search limits for tree search algorithms.
	/// </summary>
	public interface ISearchLimit
	{
		/// <summary>
		/// Returns true when the search limit is reached.
		/// </summary>
		bool Done();
	}

	/// <summary>
	/// Defines limits on tree search algorithms based on the depth of the
	/// search tree.
	/// </summary>
	public interface IDepthLimit : ISearchLimit
	{
		/// <summary>
		/// Records an increase in the depth of the search tree.
		/// </summary>
		void IncreaseDepth();

		/// <summary>
		/// Records a decrease in the depth of the search tree.
		/// </summary>
		void DecreaseDepth();
	}
}
