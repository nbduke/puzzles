using System;
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

		#region RemoveLastMappingsAdded
		[TestMethod]
		public void RemoveLastMappingsAdded_NoMappingsAdded_NoOp()
		{
			// Arrange
			var map = new CharacterMap();

			// Act
			map.RemoveLastMappingsAdded();

			// Assert
			// Pass!
		}

		[TestMethod]
		public void RemoveLastMappingsAdded_AfterAddingMappings_RemovesMostRecentSetOfKeyValuePairs()
		{
			// Arrange
			var map = new CharacterMap();
			string firstKeys = "foo";
			string firstValues = "qii";
			string secondKeys = "bar";

			map.TryAddMappings(firstKeys, firstValues);
			map.TryAddMappings(secondKeys, "xob");

			// Act
			map.RemoveLastMappingsAdded();

			// Assert
			string decodedFirstKeys = map.Decode(firstKeys);
			Assert.AreEqual(firstValues, decodedFirstKeys);
			string decodedSecondKeys = map.Decode(secondKeys);
			Assert.AreEqual(secondKeys, decodedSecondKeys);
		}

		[TestMethod]
		public void RemoveLastMappingsAdded_TwiceInARow_SecondCallIsNoOp()
		{
			// Arrange
			var map = new CharacterMap();
			string firstKeys = "foo";
			string firstValues = "qii";

			map.TryAddMappings(firstKeys, firstValues);
			map.TryAddMappings("bar", "xob");

			// Act
			map.RemoveLastMappingsAdded();
			map.RemoveLastMappingsAdded();

			// Assert
			Assert.AreEqual(firstValues, map.Decode(firstKeys));
		}
		#endregion
	}
}
