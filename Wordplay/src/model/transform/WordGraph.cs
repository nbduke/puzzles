using System;
using System.Collections.Generic;
using System.Linq;

using Tools;
using Tools.Math;
using Tools.DataStructures;

namespace Wordplay.Model.Transform
{
	/// <summary>
	/// Represents a graph whose nodes are words. Two nodes are connected
	/// if they are separated by a Levenshtein distance of one.
	/// </summary>
	public class WordGraph
	{
		private Dictionary<string, List<string>> Graph;

		/// <summary>
		/// Constructs the graph from a set of words.
		/// </summary>
		/// <remarks>
		/// The algorithm first buckets the words by length. Since only words whose
		/// lengths differ by at most one can be Levenshtein-adjacent, we only need
		/// to consider edges between words in buckets i and i+1.
		/// </remarks>
		/// <param name="allWords">the set of words that will become nodes in the graph</param>
		public WordGraph(List<string> allWords)
		{
			Validate.IsNotNull(allWords, "allWords");

			Graph = new Dictionary<string, List<string>>();
			var buckets = BucketByLength(allWords);

			foreach (var pair in buckets)
			{
				int bucketSize = pair.Key;
				var bucket = pair.Value;

				var arrangements = new Arrangement<string>(bucket);
				AddValidEdges(arrangements.GetPairs());

				if (buckets.TryGetValue(bucketSize + 1, out List<string> nextBucket))
					AddValidEdges(Combinatorics.CartesianProduct(bucket, nextBucket));
			}
		}

		/// <summary>
		/// Returns Hamming-adjacent neighbors of a word. The set of Hamming-adjacent
		/// words is a subset of the Levenshtein-adjacent words.
		/// </summary>
		/// <param name="word">the word whose neighbors are returned</param>
		public IEnumerable<string> GetHammingAdjacentWords(string word)
		{
			return GetLevenshteinAdjacentWords(word).Where(n => n.Length == word.Length);
		}

		/// <summary>
		/// Returns Levenshtein-adjacent neighbors of a word.
		/// </summary>
		/// <param name="word">the word whose neighbors are returned</param>
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
				if (!buckets.TryGetValue(length, out List<string> bucket))
				{
					bucket = new List<string>();
					buckets.Add(length, bucket);
				}

				bucket.Add(word);
			}

			return buckets;
		}

		private void AddValidEdges(IEnumerable<Tuple<string, string>> wordPairs)
		{
			foreach (var pair in wordPairs)
			{
				string a = pair.Item1;
				string b = pair.Item2;

				if (EditDistance.AreLevenshteinAdjacent(a, b))
				{
					AddEdge(a, b);
					AddEdge(b, a);
				}
			}
		}

		private void AddEdge(string node, string neighbor)
		{
			if (!Graph.TryGetValue(node, out List<string> neighbors))
			{
				neighbors = new List<string>();
				Graph.Add(node, neighbors);
			}

			neighbors.Add(neighbor);
		}
	}
}