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

		public WordTransformer(List<string> wordList)
		{
			Validate.IsNotNull(wordList, "wordList");

			var wordGraph = BuildWordGraph(wordList);
			RestrictedGraphSearcher = new AStarSearch<string>(
				word => GetHammingAdjacentChildren(word, wordGraph)
			);
			FullGraphSearcher = new AStarSearch<string>(
				word => GetLevenshteinAdjacentChildren(word, wordGraph)
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

		private static Dictionary<string, List<string>> BuildWordGraph(List<string> wordList)
		{
			var graph = new Dictionary<string, List<string>>();
			var arrangement = new Arrangement<string>(wordList);

			foreach (var pair in arrangement.GetPairs())
			{
				string a = pair.Item1;
				string b = pair.Item2;

				if (EditDistance.AreLevenshteinAdjacent(a, b))
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

		private static IEnumerable<Tuple<string, double>> GetLevenshteinAdjacentChildren(
			string word,
			Dictionary<string, List<string>> wordGraph
		)
		{
			if (wordGraph.ContainsKey(word))
				return wordGraph[word].Select(child => new Tuple<string, double>(child, 1));
			else
				return new Tuple<string, double>[] { };
		}

		private static IEnumerable<Tuple<string, double>> GetHammingAdjacentChildren(
			string word,
			Dictionary<string, List<string>> wordGraph
		)
		{
			if (wordGraph.ContainsKey(word))
				return wordGraph[word]
					.Where(s => s.Length == word.Length)
					.Select(child => new Tuple<string, double>(child, 1));
			else
				return new Tuple<string, double>[] { };
		}
	}
}