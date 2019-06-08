using System;
using System.Collections.Generic;
using System.Text;

using Tools;

namespace CryptogramSolver.Model
{
	/// <summary>
	/// Represents a bidirectional, one-to-one mapping from the English alphabet
	/// onto itself.
	/// </summary>
	public class CharacterMap
	{
		private const int ALPHABET_SIZE = 26;
		private const char UNMAPPED = '\0';

		private readonly char[] Map;
		private readonly char[] ReverseMap;

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

		/// <summary>
		/// Tries to update the map by associating a set of keys and values.
		/// </summary>
		/// <param name="keys">the keys</param>
		/// <param name="values">the values</param>
		/// <returns>true if the mapping was successful. If false,
		/// the map is not changed</returns>
		public bool TryAddMappings(string keys, string values)
		{
			var tempList = new List<KeyValuePair<char, char>>();
			return TryAddMappings(keys, values, out tempList);
		}

		/// <summary>
		/// Tries to update the map by associating a set of keys and values.
		/// </summary>
		/// <param name="keys">the keys</param>
		/// <param name="values">the values</param>
		/// <param name="mappingsAdded">the list that will contain the new mappings
		/// that were added</param>
		/// <returns>true if the mapping was successful. If false,
		/// the map is not changed</returns>
		public bool TryAddMappings(
			string keys,
			string values,
			out List<KeyValuePair<char, char>> mappingsAdded
		)
		{
			Validate.IsNotNull(keys, "keys");
			Validate.IsNotNull(values, "values");
			Validate.IsTrue(
				keys.Length == values.Length,
				"There must be exactly one key for each value."
			);

			var newMappings = new List<KeyValuePair<char, char>>();
			for (int i = 0; i < keys.Length; ++i)
			{
				char key = char.ToLower(keys[i]);
				char value = char.ToLower(values[i]);

				Validate.IsTrue(char.IsLetter(key), "Keys must be letters");
				Validate.IsTrue(char.IsLetter(value), "Values must be letters");

				char mappedKey = GetKey(value);
				char mappedValue = GetValue(key);

				if (key != value && mappedValue == UNMAPPED && mappedKey == UNMAPPED)
				{
					Insert(key, value);
					newMappings.Add(new KeyValuePair<char, char>(key, value));
				}
				else if (mappedValue != value)
				{
					RemoveMappings(newMappings);
					mappingsAdded = null;
					return false;
				}
			}

			mappingsAdded = newMappings;
			return true;
		}

		/// <summary>
		/// Removes mappings from the map.
		/// </summary>
		/// <param name="mappings">the list of key-value pairs to remove</param>
		public void RemoveMappings(IEnumerable<KeyValuePair<char, char>> mappings)
		{
			Validate.IsNotNull(mappings, "mappings");

			foreach (var pair in mappings)
			{
				Remove(pair.Key, pair.Value);
			}
		}

		/// <summary>
		/// Replaces the mapped letters in a string with their values.
		/// </summary>
		/// <param name="s">the string</param>
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