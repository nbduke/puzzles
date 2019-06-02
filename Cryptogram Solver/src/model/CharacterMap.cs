using System;
using System.Collections.Generic;
using System.Text;

using Tools;

namespace CryptogramSolver.Model
{
	class CharacterMap
	{
		private const int ALPHABET_SIZE = 26;
		private const char UNMAPPED = '\0';

		private readonly char[] Map;
		private readonly char[] ReverseMap;
		private List<KeyValuePair<char, char>> LastMappingsAdded;

		public CharacterMap()
		{
			Map = new char[ALPHABET_SIZE];
			ReverseMap = new char[ALPHABET_SIZE];

			for (int i = 0; i < ALPHABET_SIZE; ++i)
			{
				Map[i] = UNMAPPED;
				ReverseMap[i] = UNMAPPED;
			}
		}

		public bool TryAddMappings(string keys, string values)
		{
			Validate.IsNotNull(keys, "keys");
			Validate.IsNotNull(values, "values");
			Validate.IsTrue(
				keys.Length == values.Length,
				"There must be exactly one key for each value."
			);

			var mappings = new List<KeyValuePair<char, char>>();
			for (int i = 0; i < keys.Length; ++i)
			{
				char key = keys[i];
				char value = values[i];
				char mappedKey = GetKey(value);
				char mappedValue = GetValue(key);

				if (
					key != value
					&& (mappedKey == UNMAPPED || mappedKey == key)
					&& (mappedValue == UNMAPPED || mappedValue == value)
				)
				{
					Insert(key, value);
					mappings.Add(new KeyValuePair<char, char>(key, value));
				}
				else
				{
					return false;
				}
			}

			LastMappingsAdded = mappings;
			return true;
		}

		public void RemoveLastMappingsAdded()
		{
			if (LastMappingsAdded != null)
			{
				foreach (var pair in LastMappingsAdded)
				{
					Remove(pair.Key, pair.Value);
				}
			}
		}

		public string Decode(string s)
		{
			var builder = new StringBuilder(s);
			foreach (char c in s.ToLower())
			{
				if (char.IsLetter(c))
					builder.Append(GetValue(c));
				else
					builder.Append(c);
			}

			return builder.ToString();
		}

		private char GetValue(char key)
		{
			return Map[ToIndex(key)];
		}

		private char GetKey(char value)
		{
			return ReverseMap[ToIndex(value)];
		}

		private void Insert(char key, char value)
		{
			Map[ToIndex(key)] = value;
			ReverseMap[ToIndex(value)] = key;
		}

		private void Remove(char key, char value)
		{
			Map[ToIndex(key)] = UNMAPPED;
			ReverseMap[ToIndex(value)] = UNMAPPED;
		}

		private static int ToIndex(char c)
		{
			return c - 'a';
		}
	}
}