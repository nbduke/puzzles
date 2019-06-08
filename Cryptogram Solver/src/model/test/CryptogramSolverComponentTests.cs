using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptogramSolver.Model.Test
{
	[TestClass]
	public class CryptogramSolverComponentTests
	{
		[TestMethod]
		public void MultiwordPuzzleWithSolution()
		{
			// Arrange
			string expected = "the quick red fox jumps over the lazy brown dog";
			string cryptogram = EncodeByShifting(expected, 3);
			var dictionary = GetDictionaryFor(expected);

			// Act
			string actual = CryptogramSolver.Solve(cryptogram, dictionary);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void MultiwordPuzzleWithNoSolution()
		{
			// Arrange
			string plaintext = "No solution could ever possibly be found";
			string cryptogram = EncodeByShifting(plaintext, 4);
			var dictionary = GetDictionaryFor("a completely different sentence");

			// Act
			string actual = CryptogramSolver.Solve(cryptogram, dictionary);

			// Assert
			Assert.AreEqual(string.Empty, actual);
		}

		[TestMethod]
		public void PuzzleWithPunctuationAndSentenceCasing()
		{
			// Arrange
			string expected = "'Hello, world!'";
			string cryptogram = EncodeByShifting(expected, 10);
			string[] dictionary =
			{
				"hello",
				"world"
			};

			// Act
			string actual = CryptogramSolver.Solve(cryptogram, dictionary);

			// Assert
			Assert.AreEqual(expected, actual);
		}

		private static IEnumerable<string> GetDictionaryFor(string solution)
		{
			foreach (string word in solution.Split(' '))
			{
				yield return word;
			}
		}

		private static string EncodeByShifting(string plaintext, int shift)
		{
			StringBuilder builder = new StringBuilder();
			foreach (char c in plaintext)
			{
				if (char.IsLetter(c))
				{
					int encodedIndex = (c - 'a' + shift) % 26;
					char encodedChar = (char)('a' + encodedIndex);
					builder.Append(encodedChar);
				}
				else
				{
					builder.Append(c);
				}
			}
			return builder.ToString();
		}
	}
}