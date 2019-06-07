using System;
using System.Collections.Generic;
using System.Text;

using Tools;

namespace CryptogramSolver.Model
{
	public class CharacterMap
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
				char key = char.ToLower(keys[i]);
				char value = char.ToLower(values[i]);

				Validate.IsTrue(char.IsLetter(key), "Keys must be letters");
				Validate.IsTrue(char.IsLetter(value), "Values must be letters");

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
			Validate.IsNotNull(s, "s");

			var builder = new StringBuilder();
			foreach (char c in s)
			{
				if (TryGetValue(c, out char value))
				{
					if (char.IsLower(c))
						builder.Append(value);
					else
						builder.Append(char.ToUpper(value));
				}
				else
				{
					builder.Append(c);
				}
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

		private bool TryGetValue(char key, out char value)
		{
			if (char.IsLetter(key))
			{
				value = Map[ToIndex(char.ToLower(key))];
				return value != UNMAPPED;
			}
			else
			{
				value = UNMAPPED;
				return false;
			}
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