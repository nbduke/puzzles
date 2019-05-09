using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Math;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	public class WordGraph
	{
		private Dictionary<string, List<string>> Graph;

		public WordGraph(List<string> allWords)
		{
			Validate.IsNotNull(allWords, "allWords");

			Graph = new Dictionary<string, List<string>>();
			var buckets = BucketByLength(allWords);

			int minLength = buckets.Keys.Min();
			int maxLength = buckets.Keys.Max();

			for (int i = minLength; i < maxLength; ++i)
			{
				if (buckets.ContainsKey(i))
				{
					var bucket = buckets[i];
					var arrangement = new Arrangement<string>(bucket);
					AddValidConnections(arrangement.GetPairs());

					if (buckets.ContainsKey(i + 1))
					{
						var nextBucket = buckets[i + 1];
						AddValidConnections(Combinatorics.CartesianProduct(bucket, nextBucket));
					}
				}
			}
		}

		public IEnumerable<string> GetHammingAdjacentWords(string word)
		{
			if (Graph.TryGetValue(word, out List<string> neighbors))
				return neighbors.Where(n => n.Length == word.Length);
			else
				return new string[] { };
		}

		public IEnumerable<string> GetLevenshteinAdjacentWords(string word)
		{
			if (Graph.TryGetValue(word, out List<string> neighbors))
				return neighbors;
			else
				return new string[] { };
		}

		private Dictionary<int, List<string>> BucketByLength(List<string> allWords)
		{
			var buckets = new Dictionary<int, List<string>>();
			foreach (var word in allWords)
			{
				int length = word.Length;
				if (!buckets.ContainsKey(length))
					buckets[length] = new List<string>();

				buckets[length].Add(word);
			}

			return buckets;
		}

		private void AddValidConnections(IEnumerable<Tuple<string, string>> wordPairs)
		{
			foreach (var pair in wordPairs)
			{
				string a = pair.Item1;
				string b = pair.Item2;

				if (EditDistance.AreLevenshteinAdjacent(a, b))
				{
					AddConnection(a, b);
					AddConnection(b, a);
				}
			}
		}

		private void AddConnection(string node, string neighbor)
		{
			if (Graph.TryGetValue(node, out List<string> neighbors))
			{
				neighbors.Add(neighbor);
			}
			else
			{
				neighbors = new List<string>();
				neighbors.Add(neighbor);
				Graph[node] = neighbors;
			}
		}
	}
}