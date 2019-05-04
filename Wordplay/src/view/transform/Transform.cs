using System;
using System.Collections.Generic;
using System.IO;

using CommandLine;
using Wordplay.Model.Transform;
using Wordplay.View.Unscramble;

namespace Wordplay.View.Transform
{
	static class Transform
	{
		public static void Run(TransformOptions options)
		{
			if (options.SubstitutionsOnly && options.Start.Length != options.End.Length)
			{
				Console.WriteLine(
					"When only substitutions are allowed, the start and end words must be the same length."
				);
				return;
			}

			var wordList = options.SubstitutionsOnly
				? GetWordList(options.DictionaryFile, options.Start.Length)
				: GetWordList(options.DictionaryFile);

			while (true)
			{
				
			}
		}

		private static List<string> GetWordList(
			string dictionaryFilename,
			int? wordLength = null)
		{
			var words = new List<string>();
			using (var dictionaryFile = new StreamReader(dictionaryFilename))
			{
				while (!dictionaryFile.EndOfStream)
				{
					string word = dictionaryFile.ReadLine().Trim().ToLower();
					if (!string.IsNullOrEmpty(word) &&
						(!wordLength.HasValue || word.Length == wordLength.Value))
						words.Add(word);
				}
			}

			return words;
		}
	}
}