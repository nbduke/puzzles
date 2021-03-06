﻿using System;
using Tools;

namespace Player.Model
{
	/// <summary>
	/// Represents no limitation on a search algorithm. The algorithm will be able to run
	/// all the way to the leaves of the search tree.
	/// </summary>
	public class NoLimit : ISearchLimit
	{
		public bool Done() { return false; }
	}

	/// <summary>
	/// Sets a maximum depth of the search tree.
	/// </summary>
	public class DepthLimit : IDepthLimit
	{
		public readonly int MaxDepth;
		public int Depth { get; private set; }

		/// <summary>
		/// Creates a DepthLimit tailored for the initial state of the search.
		/// </summary>
		/// <param name="initialState">the initial state</param>
		public DepthLimit(GameState initialState)
		{
			Validate.IsNotNull(initialState, "initialState");

			Depth = 0;
			if (initialState.FilledCells <= 3)
				MaxDepth = 2;
			else if (initialState.FilledCells <= 12)
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

	/// <summary>
	/// Sets a maximum time duration for the search to run.
	/// </summary>
	public class TimeLimit : ISearchLimit
	{
		private readonly int DurationMs;
		private readonly DateTime StartTime;

		/// <summary>
		/// Creates a TimeLimit with a fixed duration.
		/// </summary>
		/// <param name="durationMs">the duration</param>
		public TimeLimit(int durationMs)
		{
			DurationMs = durationMs;
			StartTime = DateTime.Now;
		}

		/// <summary>
		/// Creates a TimeLimit with a duration tailored for the initial state of
		/// the search.
		/// </summary>
		/// <param name="initialState">the initial state</param>
		public TimeLimit(GameState initialState)
		{
			Validate.IsNotNull(initialState, "initialState");

			StartTime = DateTime.Now;
			if (initialState.FilledCells <= 6)
				DurationMs = 300;
			else if (initialState.FilledCells <= 10)
				DurationMs = 500;
			else if (initialState.FilledCells <= 14)
				DurationMs = 700;
			else
				DurationMs = 900;
		}

		public bool Done()
		{
			return (int)DateTime.Now.Subtract(StartTime).TotalMilliseconds >= DurationMs;
		}
	}
}
