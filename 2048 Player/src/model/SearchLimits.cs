using Tools;

namespace Player.Model
{
	/// <summary>
	/// Represents no limitation on the expectimax algorithm. The algorithm will be able to run
	/// all the way to the leaves of the decision tree.
	/// </summary>
	public class NoLimit : ISearchLimit
	{
		public bool Done() { return false; }
		public void IncreaseDepth() { }
		public void DecreaseDepth() { }
	}

	/// <summary>
	/// Sets a maximum depth of the expectimax decision tree.
	/// </summary>
	public class DepthLimit : ISearchLimit
	{
		public readonly int MaxDepth;
		public int Depth { get; private set; }

		/// <summary>
		/// Creates a DepthLimit tailored for the initial state of the search.
		/// </summary>
		/// <param name="initialState">the initial state</param>
		public DepthLimit(GameState initialState)
		{
			Depth = 0;
			if (initialState.CellsFilled <= 3)
				MaxDepth = 2;
			else if (initialState.CellsFilled <= 12)
				MaxDepth = 3;
			else
				MaxDepth = 4;
		}

		/// <summary>
		/// Creates a DepthLimit with a fixed maximum depth.
		/// </summary>
		/// <param name="maxDepth">the maximum depth</param>
		public DepthLimit(int maxDepth)
		{
			Validate.IsTrue(maxDepth > 0, "the maximum depth must be positive");

			MaxDepth = maxDepth;
			Depth = 0;
		}

		public bool Done()
		{
			return Depth >= MaxDepth;
		}

		public void IncreaseDepth()
		{
			++Depth;
		}

		public void DecreaseDepth()
		{
			--Depth;
		}
	}
}
