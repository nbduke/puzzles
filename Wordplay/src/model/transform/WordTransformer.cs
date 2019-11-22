using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Algorithms.Search;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	public class WordTransformer
	{
		private AStarSearch<string> RestrictedGraphSearcher;
		private AStarSearch<string> FullGraphSearcher;

		public WordTransformer(WordGraph graph)
		{
			Validate.IsNotNull(graph, "graph");

			RestrictedGraphSearcher = new AStarSearch<string>(
				word => AddWeights(graph.GetHammingAdjacentWords(word))
			);
			FullGraphSearcher = new AStarSearch<string>(
				word => AddWeights(graph.GetLevenshteinAdjacentWords(word))
			);
		}

		public IEnumerable<string> Transform(string start, string end, bool substitutionsOnly)
		{
			Validate.IsNotNullOrEmpty(start);
			Validate.IsNotNullOrEmpty(end);
			Validate.IsTrue(
				!substitutionsOnly || start.Length == end.Length,
				"When only substitutions are allowed, the start and end words must be the same length.");

			if (substitutionsOnly)
			{
				return RestrictedGraphSearcher.FindPath(
					start,
					end,
					word => EditDistance.CalculateHamming(word, end)
				);
			}
			else
			{
				return FullGraphSearcher.FindPath(
					start,
					end,
					word => EditDistance.CalculateLevenshtein(word, end)
				);
			}
		}

		private static IEnumerable<Tuple<string, double>> AddWeights(IEnumerable<string> children)
		{
			return children.Select(child => Tuple.Create(child, 1.0));
		}
	}
}