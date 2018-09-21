using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrosswordLottery.Model
{
	static class Constants
	{
		public const char EmptySquare = '\0';

		public static char[] Alphabet = { 'a', 'b', 'c', 'd', 'e',
										  'f', 'g', 'h', 'i', 'j',
										  'k', 'l', 'm', 'n', 'o',
										  'p', 'q', 'r', 's', 't',
										  'u', 'v', 'w', 'x', 'y', 'z'
										};

		public static uint AlphabetSize = (uint)Alphabet.Length;

		public static char[] Vowels = { 'a', 'e', 'i', 'o', 'u' };

		public static uint NumberOfVowels = (uint)Vowels.Length;

		public static char[] Consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j',
										    'k', 'l', 'm', 'n', 'p', 'q', 'r',
										    's', 't', 'v', 'w', 'x', 'y', 'z'
										  };

		public static uint NumberOfConsonants = (uint)Consonants.Length;
	}
}
