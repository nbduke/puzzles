using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SudokuSolver.Model;
using Tools.DataStructures;

namespace Test
{
	[TestClass]
	public class SudokuGridUnitTests
	{
		#region Constructor
		[TestMethod]
		public void Constructor_WithNullGrid_ThrowsArgumentNullException()
		{
			// Arrange
			Grid<int> initialValues = null;

			// Act
			Action action = () =>
			{
				var grid = new SudokuGrid(initialValues);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void Constructor_WithGridContainingInvalidValue_ThrowsArgumentException()
		{
			// Arrange
			Grid<int> initialValues = EmptyGrid();
			initialValues[4, 5] = 10; // 10 is too large

			// Act
			Action action = () =>
			{
				var grid = new SudokuGrid(initialValues);
			};

			// Assert
			Assert.ThrowsException<ArgumentException>(action);
		}

		[TestMethod]
		public void Constructor_WithGridThatIsTooLarge_ThrowsIndexOutOfRangeException()
		{
			// Arrange
			Grid<int> initialValues = new Grid<int>(100, 15);

			// Act
			Action action = () =>
			{
				var grid = new SudokuGrid(initialValues);
			};

			// Assert
			Assert.ThrowsException<IndexOutOfRangeException>(action);
		}

		[TestMethod]
		public void Constructor_WithAnyGrid_CopiesValuesIntoVariables()
		{
			// Arrange
			var initialValues = AnyInitialValues();

			// Act
			var grid = new SudokuGrid(initialValues);

			// Assert
			foreach (var pair in initialValues.GetItemLocationPairs())
			{
				var variable = grid[pair.Key];
				Assert.AreEqual(pair.Value, variable.Value);
			}
		}
		#endregion

		#region Copy constructor
		[TestMethod]
		public void CopyConstructor_WithNullGrid_ThrowsArgumentNullException()
		{
			// Arrange
			SudokuGrid original = null;

			// Act
			Action action = () =>
			{
				var copy = new SudokuGrid(original);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void CopyConstructor_WithAnyGrid_MakesDeepCopy()
		{
			// Arrange
			SudokuGrid original = new SudokuGrid(AnyInitialValues());

			// Act
			SudokuGrid copy = new SudokuGrid(original);

			// Assert
			for (int row = 0; row < SudokuGrid.GRID_SIZE; ++row)
			{
				for (int column = 0; column < SudokuGrid.GRID_SIZE; ++column)
				{
					GridCell cell = new GridCell(row, column);
					Assert.AreNotSame(original[cell], copy[cell]);
				}
			}
		}
		#endregion

		#region GetValues
		[TestMethod]
		public void GetValues_AnyGrid_ReturnsGridOfVariableValues()
		{
			// Arrange
			var initialValues = AnyInitialValues();
			var grid = new SudokuGrid(initialValues);

			// Act
			var values = grid.GetValues();

			// Assert
			CollectionAssert.AreEqual(initialValues.Flatten(), values.Flatten());
		}
		#endregion

		#region GetAllUnsetVariables
		[TestMethod]
		public void GetAllUnsetVariables_AllVariablesAreUnset_ReturnsAllVariables()
		{
			// Arrange
			var initialValues = EmptyGrid();
			var grid = new SudokuGrid(initialValues);

			// Act
			var unsetVariables = new List<Variable>(grid.GetAllUnsetVariables());

			// Assert
			Assert.AreEqual(initialValues.Cells, unsetVariables.Count);
			Assert.IsTrue(unsetVariables.All(v => !v.IsSet));
		}

		[TestMethod]
		public void GetAllUnsetVariables_NoVariablesAreUnset_ReturnsEmptyCollection()
		{
			// Arrange
			var grid = new SudokuGrid(FullGrid());

			// Act
			var unsetVariables = new List<Variable>(grid.GetAllUnsetVariables());

			// Assert
			Assert.AreEqual(0, unsetVariables.Count);
		}

		[TestMethod]
		public void GetAllUnsetVariables_SomeVariablesAreUnset_ReturnsOnlyUnsetVariables()
		{
			// Arrange
			var initialValues = AnyInitialValues();
			var grid = new SudokuGrid(initialValues);

			// Act
			var unsetVariables = new List<Variable>(grid.GetAllUnsetVariables());

			// Assert
			int unsetCount = initialValues.Count(x => x == Variable.UNSET_VALUE);
			Assert.AreEqual(unsetCount, unsetVariables.Count);
			Assert.IsTrue(unsetVariables.All(v => !v.IsSet));
		}
		#endregion

		#region GetUnsetNeighbors
		[TestMethod]
		public void GetUnsetNeighbors_WithNullVariable_ThrowsArgumentNullException()
		{
			// Arrange
			var grid = new SudokuGrid(AnyInitialValues());

			// Act
			Action action = () =>
			{
				grid.GetUnsetNeighbors(null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void GetUnsetNeighbors_WithAnyVariable_ReturnsUnsetVariablesInRowColumnAndSquare()
		{
			// Arrange
			var grid = new SudokuGrid(FullGrid());
			var variable = grid[4, 5];

			var expectedVariables = new Variable[]
			{
				grid[4, 6], // same row
				grid[1, 5], // same column
				grid[3, 3], // same square
				grid[5, 5]  // same column and square
			};

			foreach (var neighbor in expectedVariables)
			{
				neighbor.Unset();
			}

			// Act
			var unsetNeighbors = new List<Variable>(grid.GetUnsetNeighbors(variable));

			// Assert
			CollectionAssert.AreEquivalent(expectedVariables, unsetNeighbors);
		}

		[TestMethod]
		public void GetUnsetNeighbors_VariableIsUnset_ScreensOutVariable()
		{
			// Arrange
			var grid = new SudokuGrid(FullGrid());
			var variable = grid[0, 2];
			variable.Unset();

			var neighbors = new Variable[]
			{
				grid[0, 5], // same row
				grid[3, 2], // same column
				grid[1, 0], // same square
				grid[2, 2]  // same column and square
			};

			foreach (var neighbor in neighbors)
			{
				neighbor.Unset();
			}

			// Act
			var unsetNeighbors = new List<Variable>(grid.GetUnsetNeighbors(variable));

			// Assert
			CollectionAssert.DoesNotContain(unsetNeighbors, variable);
		}
		#endregion

		#region GetSetNeighbors
		[TestMethod]
		public void GetSetNeighbors_WithNullVariable_ThrowsArgumentNullException()
		{
			// Arrange
			var grid = new SudokuGrid(AnyInitialValues());

			// Act
			Action action = () =>
			{
				grid.GetSetNeighbors(null);
			};

			// Assert
			Assert.ThrowsException<ArgumentNullException>(action);
		}

		[TestMethod]
		public void GetSetNeighbors_WithAnyVariable_ReturnsSetVariablesInRowColumnAndSquare()
		{
			// Arrange
			var grid = new SudokuGrid(EmptyGrid());
			var variable = grid[8, 8];

			var expectedVariables = new Variable[]
			{
				grid[8, 3], // same row
				grid[1, 8], // same column
				grid[7, 7], // same square
				grid[8, 6]  // same row and square
			};

			foreach (var neighbor in expectedVariables)
			{
				neighbor.Value = 2;
			}

			// Act
			var setNeighbors = new List<Variable>(grid.GetSetNeighbors(variable));

			// Assert
			CollectionAssert.AreEquivalent(expectedVariables, setNeighbors);
		}

		[TestMethod]
		public void GetSetNeighbors_VariableIsSet_ReturnsVariableWithNeighbors()
		{
			// Arrange
			var grid = new SudokuGrid(EmptyGrid());
			var variable = grid[6, 1];
			variable.Value = 4;

			var neighbors = new Variable[]
			{
				grid[6, 6], // same row
				grid[2, 1], // same column
				grid[7, 0], // same square
				grid[6, 0]  // same row and square
			};

			foreach (var neighbor in neighbors)
			{
				neighbor.Value = 7;
			}

			// Act
			var setNeighbors = new List<Variable>(grid.GetSetNeighbors(variable));

			// Assert
			CollectionAssert.DoesNotContain(setNeighbors, variable);
		}
		#endregion

		#region Helpers
		private static Grid<int> EmptyGrid()
		{
			return new Grid<int>(
				SudokuGrid.GRID_SIZE,
				SudokuGrid.GRID_SIZE,
				Variable.UNSET_VALUE
			);
		}

		private static Grid<int> FullGrid()
		{
			var grid = EmptyGrid();
			int value = 0;

			foreach (var pair in grid.GetItemLocationPairs())
			{
				grid[pair.Key] = value + 1;
				value = (value + 1) % Variable.MAX_POSSIBLE_VALUE;
			}

			return grid;
		}

		private static Grid<int> AnyInitialValues()
		{
			var values = EmptyGrid();
			values[0, 4] = 5;
			values[1, 3] = 2;
			values[1, 5] = 1;
			values[3, 0] = 1;
			values[3, 8] = 3;
			values[5, 5] = 4;
			values[5, 6] = 7;
			values[6, 0] = 2;
			values[7, 6] = 8;
			values[8, 2] = 6;
			values[8, 7] = 4;
			return values;
		}
		#endregion
	}
}