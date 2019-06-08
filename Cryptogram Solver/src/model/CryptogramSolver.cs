using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using Tools;

namespace CryptogramSolver.Model
{
	public static class CryptogramSolver
	{
		public static string Solve(string cryptogram, IEnumerable<string> dictionary)
		{
			Validate.IsNotNullOrEmpty(cryptogram);
			Validate.IsNotNull(dictionary, "dictionary");

			var cryptogramWords = GetWords(cryptogram);
			var wordsWithCandidates = GetCandidatesForWords(cryptogramWords, dictionary);

			if (wordsWithCandidates == null)
				return "";

			dictionary = null; // release this as we don't need it anymore
			var orderedWords = wordsWithCandidates.Keys.OrderBy(
				word => wordsWithCandidates[word].Count
			).ToList();

			var characterMap = new CharacterMap();
			if (SolveHelper(characterMap, wordsWithCandidates, orderedWords, 0))
				return characterMap.Decode(cryptogram);
			else
				return "";
		}

		private static IEnumerable<string> GetWords(string cryptogram)
		{
			string trimmed = cryptogram.Trim();
			string noSpecialChars = Regex.Replace(trimmed, @"[^\w\- ]", "");
			string[] words = Regex.Split(noSpecialChars, @"[\- ]");
			return words.Where(w => w.Length > 0);
		}

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
							partition.Where(c => IsCandidate(word, c)).ToList()
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

		private static bool IsCandidate(string word, string candidate)
		{
			var map = new CharacterMap();
			return map.TryAddMappings(word, candidate);
		}

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