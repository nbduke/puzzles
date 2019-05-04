using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Algorithms.Search;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	public static class WordTransformSearchFactory
	{
		public static AStarSearch<string> Create(
			List<string> allWords,
			string goalWord,
			bool onlyAllowSubstitutions
		)
		{
			Validate.IsNotNull(allWords, "allWords");
			Validate.IsNotNullOrEmpty(goalWord);

			return onlyAllowSubstitutions
				? CreateRestrictedSearch(allWords, goalWord)
				: CreateGeneralSearch(allWords, goalWord);
		}

		private static AStarSearch<string> CreateRestrictedSearch(List<string> allWords, string goalWord)
		{
			var wordGraph = BuildWordGraph(allWords, EditDistance.CountMismatches);
			return new AStarSearch<string>(
				word => GetChildren(word, wordGraph),
				word => EditDistance.CountMismatches(word, goalWord)
			);
		}

		private static AStarSearch<string> CreateGeneralSearch(List<string> allWords, string goalWord)
		{
			var wordGraph = BuildWordGraph(allWords, EditDistance.Calculate);
			return new AStarSearch<string>(
				word => GetChildren(word, wordGraph),
				word => EditDistance.Calculate(word, goalWord)
			);
		}

		private static Dictionary<string, List<string>> BuildWordGraph(
			List<string> allWords,
			Func<string, string, int> getEditDistance
		)
		{
			var graph = new Dictionary<string, List<string>>();
			var arrangement = new Arrangement<string>(allWords);

			foreach (var pair in arrangement.GetPairs())
			{
				string a = pair.Item1;
				string b = pair.Item2;
				int editDistance = getEditDistance(a, b);

				if (editDistance <= 1)
				{
					if (!graph.ContainsKey(a))
						graph[a] = new List<string>();
					if (!graph.ContainsKey(b))
						graph[b] = new List<string>();

					graph[a].Add(b);
					graph[b].Add(a);
				}
			}

			return graph;
		}

		private static IEnumerable<Tuple<string, double>> GetChildren(
			string word,
			Dictionary<string, List<string>> wordGraph
		)
		{
			if (wordGraph.ContainsKey(word))
				return wordGraph[word].Select(child => new Tuple<string, double>(child, 1));
			else
				return new Tuple<string, double>[] { };
		}
	}
}