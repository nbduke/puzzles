using System.Collections.Generic;

namespace CrosswordLottery.Model
{
	static class CrosswordAnalysis
	{
		public static uint CountRevealedWords(this CrosswordContent content, IEnumerable<char> givenCharacters)
		{
			SortedSet<char> sortedGivenChars = new SortedSet<char>(givenCharacters);
			uint count = 0;

			foreach (string word in content.WordList)
			{
				bool wordRevealed = true;
				foreach (char c in word)
				{
					if (!sortedGivenChars.Contains(c))
					{
						wordRevealed = false;
						break;
					}
				}

				if (wordRevealed)
					++count;
			}

			return count;
		}

		public static uint CountExcludedWords(this CrosswordContent crossword, IEnumerable<char> excludedCharacters)
		{
			SortedSet<char> sortedExcludedChars = new SortedSet<char>(excludedCharacters);
			uint count = 0;

			foreach (string word in crossword.WordList)
			{
				foreach (char c in word)
				{
					if (sortedExcludedChars.Contains(c))
					{
						++count;
						break;
					}
				}
			}

			return count;
		}
	}
}
