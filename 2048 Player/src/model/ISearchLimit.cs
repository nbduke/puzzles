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
		/// Advances to the next layer of the search.
		/// </summary>
		void NextSearch();

		/// <summary>
		/// Returns a copy of this search limit instance.
		/// </summary>
		ISearchLimit Copy();
	}
}
