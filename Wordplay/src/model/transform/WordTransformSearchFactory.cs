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
			List<string> wordList,
			string goalWord,
			bool substitutionsOnly
		)
		{
			Validate.IsNotNull(wordList, "allWords");
			Validate.IsNotNullOrEmpty(goalWord);

			return substitutionsOnly
				? CreateRestrictedSearch(wordList, goalWord)
				: CreateGeneralSearch(wordList, goalWord);
		}

		private static AStarSearch<string> CreateRestrictedSearch(List<string> wordList, string goalWord)
		{
			var wordGraph = BuildWordGraph(wordList, (a, b) =>
			{
				return a.Length == b.Length && EditDistance.CountMismatches(a, b) <= 1;
			});
			return new AStarSearch<string>(
				word => GetChildren(word, wordGraph),
				word => EditDistance.CountMismatches(word, goalWord)
			);
		}

		private static AStarSearch<string> CreateGeneralSearch(List<string> wordList, string goalWord)
		{
			var wordGraph = BuildWordGraph(wordList, (a, b) =>
			{
				return Math.Abs(a.Length - b.Length) <= 1 && EditDistance.Calculate(a, b) <= 1;
			});
			return new AStarSearch<string>(
				word => GetChildren(word, wordGraph),
				word => EditDistance.Calculate(word, goalWord)
			);
		}

		private static Dictionary<string, List<string>> BuildWordGraph(
			List<string> wordList,
			Func<string, string, bool> shouldConnectWords
		)
		{
			var graph = new Dictionary<string, List<string>>();
			var arrangement = new Arrangement<string>(wordList);

			foreach (var pair in arrangement.GetPairs())
			{
				string a = pair.Item1;
				string b = pair.Item2;

				if (shouldConnectWords(a, b))
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