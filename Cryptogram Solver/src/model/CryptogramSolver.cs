using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

using Tools;

namespace CryptogramSolver.Model
{
	public class CryptogramSolver
	{
		private readonly Dictionary<int, HashSet<string>> WordPartitions;

		public static string Solve(string cryptogram, IEnumerable<string> wordList)
		{
			var wordPartitions = new Dictionary<int, HashSet<string>>();
			foreach (string word in wordList)
			{
				if (!wordPartitions.TryGetValue(word.Length, out HashSet<string> partition))
					partition = new HashSet<string>();

				partition.Add(word);
			}

			var solver = new CryptogramSolver(wordPartitions);
			return solver.Solve(cryptogram);
		}

		public CryptogramSolver(Dictionary<int, HashSet<string>> wordPartitions)
		{
			Validate.IsNotNull(wordPartitions, "wordPartitions");
			WordPartitions = wordPartitions;
		}

		public string Solve(string cryptogram)
		{
			Validate.IsNotNullOrEmpty(cryptogram);

			var cryptogramWords = GetWords(cryptogram);
			if (!PartitionsExistForEachWord(cryptogramWords))
				return "";

			var wordsWithCandidates = GetCandidatesForWords(cryptogramWords);
			var orderedWords = wordsWithCandidates.Keys.OrderBy(
				word => wordsWithCandidates[word].Count
			).ToList();

			var characterMap = new CharacterMap();
			if (SolveHelper(
				characterMap,
				wordsWithCandidates,
				orderedWords,
				0
			))
				return characterMap.Decode(cryptogram);
			else
				return "";
		}

		private static IEnumerable<string> GetWords(string cryptogram)
		{
			string preprocessed = cryptogram.ToLower().Trim();
			string noSpecialChars = Regex.Replace(preprocessed, @"[^\w\- ]", "");
			string[] words = Regex.Split(noSpecialChars, @"[\- ]");
			return words.Where(w => w.Length > 0);
		}

		private bool PartitionsExistForEachWord(IEnumerable<string> words)
		{
			foreach (string word in words)
			{
				if (!WordPartitions.ContainsKey(word.Length))
					return false;
			}

			return true;
		}

		private Dictionary<string, List<string>> GetCandidatesForWords(
			IEnumerable<string> cryptogramWords
		)
		{
			var candidates = new Dictionary<string, List<string>>();
			foreach (string word in cryptogramWords)
			{
				if (!candidates.ContainsKey(word))
				{
					var partition = WordPartitions[word.Length];
					candidates.Add(
						word,
						partition.Where(c => IsCandidate(word, c)).ToList()
					);
				}
			}

			return candidates;
		}

		private static bool IsCandidate(string word, string candidate)
		{
			var map = new CharacterMap();
			return map.TryAddMappings(word, candidate);
		}

		private bool SolveHelper(
			CharacterMap characterMap,
			Dictionary<string, List<string>> wordsWithCandidates,
			List<string> words,
			int nextWordIndex
		)
		{
			if (nextWordIndex >= words.Count)
				return true;

			string encryptedWord = words[nextWordIndex];
			foreach (string candidate in wordsWithCandidates[encryptedWord])
			{
				if (characterMap.TryAddMappings(encryptedWord, candidate))
				{
					if (SolveHelper(
						characterMap,
						wordsWithCandidates,
						words,
						++nextWordIndex
					))
						return true;
					else
						characterMap.RemoveLastMappingsAdded();
				}
			}

			return false;
		}
	}
}