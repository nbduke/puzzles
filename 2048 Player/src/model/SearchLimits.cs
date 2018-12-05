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
		public void NextSearch() { }
		public ISearchLimit Copy() { return new NoLimit(); }
	}

	/// <summary>
	/// Sets a maximum depth of the expectimax decision tree.
	/// </summary>
	public class DepthLimit : ISearchLimit
	{
		public readonly int MaxDepth;
		public int Depth { get; private set; }

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

		public void NextSearch()
		{
			++Depth;
		}

		public ISearchLimit Copy()
		{
			DepthLimit dl = new DepthLimit(MaxDepth);
			dl.Depth = Depth;
			return dl;
		}
	}
}
