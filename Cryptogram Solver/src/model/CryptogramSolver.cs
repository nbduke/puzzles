using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using Tools;

namespace CryptogramSolver.Model
{
	/// <summary>
	/// The core cryptogram solving algorithm.
	/// </summary>
	/// <remarks>
	/// The algorithm is a modified constraint satisfaction problem solver using
	/// heuristic-guided backtracking search. The variables are the unique words
	/// of the puzzle, and the values are the respective dictionary words that match
	/// the length and character pattern of the word. Constraints are propagated
	/// using a CharacterMap that tracks the mappings between letters. Once all
	/// encrypted words have been matched to dictionary words, the map can be used
	/// to decode the original cryptogram.
	/// </remarks>
	public static class CryptogramSolver
	{
		/// <summary>
		/// Solves a cryptogram puzzle instance.
		/// </summary>
		/// <param name="cryptogram">the cryptogram instance</param>
		/// <param name="dictionary">the list of words in the dictionary</param>
		public static string Solve(string cryptogram, IEnumerable<string> dictionary)
		{
			Validate.IsNotNullOrEmpty(cryptogram);
			Validate.IsNotNull(dictionary, "dictionary");

			var cryptogramWords = GetWords(cryptogram);
			var wordsWithCandidates = GetCandidatesForWords(cryptogramWords, dictionary);

			if (wordsWithCandidates == null)
				return "";

			dictionary = null; // release this as we don't need it anymore

			// Sort the words by number of candidates. Words will be assigned in that order,
			// approximating a minimum remaining values heuristic.
			var orderedWords = wordsWithCandidates.Keys.OrderBy(
				word => wordsWithCandidates[word].Count
			).ToList();

			var characterMap = new CharacterMap();
			if (SolveHelper(characterMap, wordsWithCandidates, orderedWords, 0))
				return characterMap.Decode(cryptogram);
			else
				return "";
		}

		/*
		 * Removes special characters, trims whitespace, and splits along spaces
		 * and dashes.
		 */
		private static IEnumerable<string> GetWords(string cryptogram)
		{
			string trimmed = cryptogram.Trim();
			string noSpecialChars = Regex.Replace(trimmed, @"[^\w\- ]", "");
			string[] words = Regex.Split(noSpecialChars, @"[\- ]");
			return words.Where(w => w.Length > 0);
		}

		/*
		 * For each word in the cryptogram, finds the set of dictionary words that
		 * could be associated based on length and character pattern.
		 */
		private static Dictionary<string, List<string>> GetCandidatesForWords(
			IEnumerable<string> cryptogramWords,
			IEnumerable<string> dictionary
		)
		{
			var partitionedDictionary = PartitionDictionary(dictionary);
			var wordsWithCandidates = new Dictionary<string, List<string>>();

			foreach (string word in cryptogramWords)
			{
				if (!wordsWithCandidates.ContainsKey(word))
				{
					if (partitionedDictionary.TryGetValue(
						word.Length,
						out HashSet<string> partition
					))
					{
						wordsWithCandidates.Add(
							word,
							partition.Where(c => MatchesPattern(word, c)).ToList()
						);
					}
					else
					{
						return null;
					}
				}
			}

			return wordsWithCandidates;
		}

		/*
		 * Partitions the dictionary by word length.
		 */
		private static Dictionary<int, HashSet<string>> PartitionDictionary(
			IEnumerable<string> dictionary
		)
		{
			var partitionedDictionary = new Dictionary<int, HashSet<string>>();
			foreach (string word in dictionary)
			{
				if (!partitionedDictionary.TryGetValue(word.Length, out HashSet<string> partition))
				{
					partition = new HashSet<string>();
					partitionedDictionary.Add(word.Length, partition);
				}

				partition.Add(word);
			}
			return partitionedDictionary;
		}

		/*
		 * Determines whether a word can be mapped to another word based on its
		 * character pattern.
		 *
		 * For example, "pool" cannot be mapped to "xztj" because the latter does
		 * not contain two repeated letters in the middle. "xyyj" would work, however.
		 */
		private static bool MatchesPattern(string word, string candidate)
		{
			var map = new CharacterMap();
			return map.TryAddMappings(word, candidate);
		}

		/*
		 * Runs the backtracking search.
		 */
		private static bool SolveHelper(
			CharacterMap characterMap,
			Dictionary<string, List<string>> wordsWithCandidates,
			List<string> cryptogramWords,
			int nextWordIndex
		)
		{
			if (nextWordIndex >= cryptogramWords.Count)
				return true;

			string cryptogramWord = cryptogramWords[nextWordIndex];
			foreach (string candidate in wordsWithCandidates[cryptogramWord])
			{
				if (characterMap.TryAddMappings(
					cryptogramWord,
					candidate,
					out List<KeyValuePair<char, char>> newMappings
				))
				{
					if (SolveHelper(
						characterMap,
						wordsWithCandidates,
						cryptogramWords,
						nextWordIndex + 1
					))
						return true;
					else
						characterMap.RemoveMappings(newMappings);
				}
			}

			return false;
		}
	}
}