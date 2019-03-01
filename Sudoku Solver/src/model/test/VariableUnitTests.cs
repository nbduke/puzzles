using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SudokuSolver.Model;
using Tools.DataStructures;

namespace Test
{
	[TestClass]
	public class VariableUnitTests
	{
		#region Constructor
		[TestMethod]
		public void Constructor_WithoutValue_SetsLocationAndValueToUnsetValue()
		{
			// Act
			var variable = new Variable(AnyLocation);

			// Assert
			Assert.AreEqual(AnyLocation, variable.Location);
			Assert.AreEqual(Variable.UNSET_VALUE, variable.Value);
		}

		[TestMethod]
		public void Constructor_WithValidValue_SetsLocationAndValue()
		{
			// Arrange
			int anyValue = 7;

			// Act
			var variable = new Variable(AnyLocation, anyValue);

			// Assert
			Assert.AreEqual(AnyLocation, variable.Location);
			Assert.AreEqual(anyValue, variable.Value);
		}

		[TestMethod]
		public void Constructor_WithNegativeValue_ThrowsArgumentException()
		{
			// Act
			Action action = () =>
			{
				var variable = new Variable(new GridCell(), -1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void Constructor_WithTooLargeValue_ThrowsArgumentException()
		{
			// Act
			Action action = () =>
			{
				var variable = new Variable(new GridCell(), Variable.MAX_POSSIBLE_VALUE + 1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}
		#endregion

		#region Copy Constructor
		[TestMethod]
		public void CopyConstructor_WithNullVariable_ThrowsArgumentNullException()
		{
			// Act
			Action action = () =>
			{
				var variable = new Variable(null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void CopyConstructor_WithAnyVariable_MakesDeepCopy()
		{
			// Arrange
			int value = 7;
			int valueToRemoveFromCopy = 3;
			var original = new Variable(AnyLocation, value);

			// Act
			var copy = new Variable(original);
			copy.RemovePossibleValue(valueToRemoveFromCopy);

			// Assert
			Assert.AreEqual(original.Location, copy.Location);
			Assert.AreEqual(original.Value, copy.Value);
			Assert.IsTrue(original.IsPossibleValue(valueToRemoveFromCopy));
		}
		#endregion

		#region IsSet
		[TestMethod]
		public void IsSet_ConstructedWithNonzeroValue_ReturnsTrue()
		{
			// Arrange
			var variable = new Variable(AnyLocation, 1);

			// Act
			bool isSet = variable.IsSet;

			// Assert
			Assert.IsTrue(isSet);
		}

		[TestMethod]
		public void IsSet_ConstructedWithZero_ReturnsFalse()
		{
			// Arrange
			var variable = new Variable(AnyLocation, Variable.UNSET_VALUE);

			// Act
			bool isSet = variable.IsSet;

			// Assert
			Assert.IsFalse(isSet);
		}

		[TestMethod]
		public void IsSet_AfterChangingValueToNonzero_ReturnsTrue()
		{
			// Arrange
			var variable = new Variable(AnyLocation, Variable.UNSET_VALUE);
			variable.Value = 2;

			// Act
			bool isSet = variable.IsSet;

			// Assert
			Assert.IsTrue(isSet);
		}
		#endregion

		#region Unset
		[TestMethod]
		public void Unset_AtAnyTime_MakesIsSetFalse()
		{
			// Arrange
			var variable = new Variable(AnyLocation, 6);

			// Act
			variable.Unset();

			// Assert
			Assert.IsFalse(variable.IsSet);
		}
		#endregion

		#region PossibleValuesCount and GetPossibleValues
		[TestMethod]
		public void PossibleValuesCount_BeforeRemovingAnyPossibleValues_ReturnsMaxPossibleValue()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			int possibleValuesCount = variable.PossibleValuesCount;

			// Assert
			Assert.AreEqual(Variable.MAX_POSSIBLE_VALUE, possibleValuesCount);
		}

		[TestMethod]
		public void GetPossibleValues_BeforeRemovingAnyPossibleValues_ReturnsAllPossibleValues()
		{
			// Arrange
			var variable = new Variable(AnyLocation);
			var expectedValues = new List<int>(AllPossibleValues());

			// Act
			var possibleValues = new List<int>(variable.GetPossibleValues());

			// Assert
			CollectionAssert.AreEqual(expectedValues, possibleValues);
		}
		#endregion

		#region IsPossibleValue
		[TestMethod]
		public void IsPossibleValue_WithUnsetValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.IsPossibleValue(Variable.UNSET_VALUE);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void IsPossibleValue_WithNegativeValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.IsPossibleValue(-1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void IsPossibleValue_WithTooLargeValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.IsPossibleValue(Variable.MAX_POSSIBLE_VALUE + 1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void IsPossibleValue_BeforeRemovingAnyPossibleValues_ReturnsTrue()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			bool isPossible = variable.IsPossibleValue(4);

			// Assert
			Assert.IsTrue(isPossible);
		}
		#endregion

		#region RemovePossibleValue
		[TestMethod]
		public void RemovePossibleValue_WithUnsetValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.RemovePossibleValue(Variable.UNSET_VALUE);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void RemovePossibleValue_WithNegativeValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.RemovePossibleValue(-1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void RemovePossibleValue_WithTooLargeValue_ThrowsArgumentException()
		{
			// Arrange
			var variable = new Variable(AnyLocation);

			// Act
			Action action = () =>
			{
				variable.RemovePossibleValue(Variable.MAX_POSSIBLE_VALUE + 1);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void RemovePossibleValue_WithCurrentlyPossibleValue_DecreasesPossibleValuesCountByOne()
		{
			// Arrange
			var variable = new Variable(AnyLocation);
			int currentPossibleValuesCount = variable.PossibleValuesCount;

			// Act
			variable.RemovePossibleValue(4);

			// Assert
			Assert.AreEqual(currentPossibleValuesCount - 1, variable.PossibleValuesCount);
		}

		[TestMethod]
		public void RemovePossibleVale_WithCurrentlyImpossibleValue_DoesNotChangePossibleValuesCount()
		{
			// Arrange
			var variable = new Variable(AnyLocation);
			int valueToRemove = 1;
			variable.RemovePossibleValue(valueToRemove);
			int currentPossibleValuesCount = variable.PossibleValuesCount;

			// Act
			variable.RemovePossibleValue(valueToRemove);

			// Assert
			Assert.AreEqual(currentPossibleValuesCount, variable.PossibleValuesCount);
		}

		[TestMethod]
		public void RemovePossibleValue_WithCurrentlyPossibleValue_RemovesFromPossibleValues()
		{
			// Arrange
			var variable = new Variable(AnyLocation);
			int valueToRemove = 3;
			var expectedValues = new List<int>(AllPossibleValues().Where(v => v != valueToRemove));

			// Act
			variable.RemovePossibleValue(valueToRemove);

			// Assert
			var actualValues = new List<int>(variable.GetPossibleValues());
			CollectionAssert.AreEqual(expectedValues, actualValues);
		}

		[TestMethod]
		public void RemovePossibleValue_WithCurrentlyPossibleValue_IsPossibleValueReturnsFalse()
		{
			// Arrange
			var variable = new Variable(AnyLocation);
			int valueToRemove = 8;

			// Act
			variable.RemovePossibleValue(valueToRemove);

			// Assert
			Assert.IsFalse(variable.IsPossibleValue(valueToRemove));
		}
		#endregion

		#region Helpers
		private static readonly GridCell AnyLocation = new GridCell(2, 3);

		private static IEnumerable<int> AllPossibleValues()
		{
			for (int i = 1; i <= Variable.MAX_POSSIBLE_VALUE; ++i)
			{
				yield return i;
			}
		}
		#endregion
	}
}
