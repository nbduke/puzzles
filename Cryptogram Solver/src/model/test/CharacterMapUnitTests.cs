using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CryptogramSolver.Model.Test
{
	[TestClass]
	public class CharacterMapUnitTests
	{
		#region TryAddMappings
		[TestMethod]
		public void TryAddMappings_KeysStringIsNull_ThrowsArgumentNullException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.TryAddMappings(null, "anyValues");
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void TryAddMappings_ValuesStringIsNull_ThrowsArgumentNullException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.TryAddMappings("anyValues", null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void TryAddMappings_DifferentNumberOfKeysAndValues_ThrowsArgumentException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.TryAddMappings("xyz", "abcd");
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void TryAddMappings_KeysStringContainsNonLetterCharacters_ThrowsArgumentException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.TryAddMappings("key5", "values");
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void TryAddMappings_ValuesStringContainsNonLetterCharacters_ThrowsArgumentException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.TryAddMappings("keys", "v@lues");
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void TryAddMappings_OneKeyValuePairIsIdentical_ReturnsFalse()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			bool result = map.TryAddMappings("frog", "xyoc");

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryAddMappings_DuplicateKeysWithoutDuplicateValues_ReturnsFalse()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			bool result = map.TryAddMappings("food", "wxyz");

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryAddMappings_DuplicateValuesWithoutDuplicateKeys_ReturnsFalse()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			bool result = map.TryAddMappings("cats", "eeij");

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryAddMappings_ParityWithKeysAndValues_ReturnsTrue()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			bool result = map.TryAddMappings("logo", "cviv");

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TryAddMappings_KeyIsAlreadyMappedToDifferentValue_ReturnsFalse()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			map.TryAddMappings("cat", "dkj");
			bool result = map.TryAddMappings("tar", "jpv"); // a mapped to k first, then p

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryAddMappings_ValueIsAlreadyMappedToDifferentKey_ReturnsFalse()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			map.TryAddMappings("dog", "rxu");
			bool result = map.TryAddMappings("bee", "xcc"); // x is mapped to o first, then b

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void TryAddMappings_FailsWithEntriesInTheMap_MapDoesNotChange()
		{
			// Arrange
			var map = new CharacterMap();
			string firstKeys = "frogs";
			string firstValues = "vbnde";
			string secondKeys = "rugs";
			string secondValues = "btdf"; // s in "rugs" already mapped to e

			// Act
			map.TryAddMappings(firstKeys, firstValues);
			string decodedSecondKeys = map.Decode(secondKeys);
			bool result = map.TryAddMappings(secondKeys, secondValues);

			// Assert
			Assert.IsFalse(result);
			Assert.AreEqual(firstValues, map.Decode(firstKeys));
			Assert.AreEqual(decodedSecondKeys, map.Decode(secondKeys));
		}

		[TestMethod]
		public void TryAddMappings_WithOutParamAndFails_OutParamSetToNull()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			bool result = map.TryAddMappings(
				"howdy",
				"niilb",
				out List<KeyValuePair<char, char>> mappingsAdded
			);

			// Assert
			Assert.IsFalse(result);
			Assert.IsNull(mappingsAdded);
		}

		[TestMethod]
		public void TryAddMappings_WithOutParamAndSucceeds_OutParamHoldsMappingsJustAdded()
		{
			// Arrange
			var map = new CharacterMap();
			map.TryAddMappings("foo", "xyy");

			// Act
			bool result = map.TryAddMappings(
				"loud",
				"typg",
				out List<KeyValuePair<char, char>> mappingsAdded
			);

			// Assert
			KeyValuePair<char, char>[] expectedMappings =
			{
				new KeyValuePair<char, char>('l', 't'),
				new KeyValuePair<char, char>('u', 'p'),
				new KeyValuePair<char, char>('d', 'g')
			};
			Assert.IsTrue(result);
			CollectionAssert.AreEqual(expectedMappings, mappingsAdded);
		}
		#endregion

		#region Decode
		[TestMethod]
		public void Decode_StringIsNull_ThrowsArgumentNullException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.Decode(null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void Decode_StringIsEmpty_ReturnsEmptyString()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			string result = map.Decode("");

			// Assert
			Assert.AreEqual(string.Empty, result);
		}

		[TestMethod]
		public void Decode_StringDoesNotContainLetters_ReturnsOriginalString()
		{
			// Arrange
			var map = new CharacterMap();
			string input = "@37*$!2";

			// Act
			string result = map.Decode(input);

			// Assert
			Assert.AreEqual(input, result);
		}

		[TestMethod]
		public void Decode_StringDoesNotContainMappedLetters_ReturnsOriginalString()
		{
			// Arrange
			var map = new CharacterMap();
			map.TryAddMappings("foo", "xrr");
			string input = "laugh";

			// Act
			string result = map.Decode(input);

			// Assert
			Assert.AreEqual(input, result);
		}

		[TestMethod]
		public void Decode_StringContainsSomeMappedLetters_ReturnsOriginalStringWithMappedLettersDecoded()
		{
			// Arrange
			var map = new CharacterMap();
			map.TryAddMappings("foo", "xrr");
			string input = "#follow";

			// Act
			string result = map.Decode(input);

			// Assert
			Assert.AreEqual("#xrllrw", result);
		}

		[TestMethod]
		public void Decode_StringContainsSomeUppercaseMappedLetters_DecodesMappedLettersToCorrectCase()
		{
			// Arrange
			var map = new CharacterMap();
			map.TryAddMappings("cat", "zyo");
			string input = "CaT";

			// Act
			string result = map.Decode(input);

			// Assert
			Assert.AreEqual("ZyO", result);
		}
		#endregion

		#region RemoveMappings
		[TestMethod]
		public void RemoveMappings_MappingsListIsNull_ThrowsNullArgumentException()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			Action action = () =>
			{
				map.RemoveMappings(null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void RemoveMappings_WithKeysAndValuesNotInTheMap_NoOp()
		{
			// Arrange
			var map = new CharacterMap();
			string firstKeys = "xz";
			string firstValues = "mn";
			map.TryAddMappings(firstKeys, firstValues);

			var mappings = new KeyValuePair<char, char>[]
			{
				new KeyValuePair<char, char>('a', 'b'),
				new KeyValuePair<char, char>('c', 'd')
			};

			// Act
			map.RemoveMappings(mappings);

			// Assert
			Assert.AreEqual(firstValues, map.Decode(firstKeys));
		}

		[TestMethod]
		public void RemoveMappings_WithMappingsFromTryAddMappings_RemovesOnlyNewMappings()
		{
			// Arrange
			var map = new CharacterMap();
			string firstKeys = "dog";
			string firstValues = "brv";
			string secondKeys = "top";
			string secondValues = "wrz";

			map.TryAddMappings(firstKeys, firstValues);
			string decodedSecondKeys = map.Decode(secondKeys);

			map.TryAddMappings(
				secondKeys,
				secondValues,
				out List<KeyValuePair<char, char>> mappingsAdded
			);

			// Act
			map.RemoveMappings(mappingsAdded);

			// Assert
			Assert.AreEqual(firstValues, map.Decode(firstKeys));
			Assert.AreEqual(decodedSecondKeys, map.Decode(secondKeys));
		}
		#endregion
	}
}
