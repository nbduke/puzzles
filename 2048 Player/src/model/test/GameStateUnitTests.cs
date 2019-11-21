using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Player.Model;

namespace Test
{
	[TestClass]
	public class GameStateUnitTests
	{
		#region IsActionLegal
		[TestMethod]
		public void IsActionLegal_WithNoAction_ReturnsFalse()
		{
			// Arrange
			var state = GameSimulator.RandomInitialState();

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.NoAction);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionAndStateIsAWin_ReturnsFalse()
		{
			// Arrange
			var state = new GameState(0); // already in the win state

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Left);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionAndStateIsEmpty_ReturnsFalse()
		{
			// Arrange
			var state = new GameState();

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Down);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionWhereNoMovementIsPossible_ReturnsFalse()
		{
			// Arrange
			var state = new GameState();
			state.AddTile(new Tile(0, 0, 2));
			state.AddTile(new Tile(0, 3, 2));

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Up);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionWhereTilesCouldNotBeCombined_ReturnsFalse()
		{
			// Arrange
			var state = new GameState();
			state.AddTile(new Tile(1, 2, 2));
			state.AddTile(new Tile(1, 3, 4));

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Right);

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionWhereMovementIsPossible_ReturnsTrue()
		{
			// Arrange
			var state = new GameState();
			state.AddTile(new Tile(2, 2, 2));

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Down);

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void IsActionLegal_AnyActionWhereTilesCouldBeCombined_ReturnsTrue()
		{
			// Arrange
			var state = new GameState();
			state.AddTile(new Tile(0, 2, 4));
			state.AddTile(new Tile(0, 0, 4));

			// Act
			bool result = state.IsActionLegal(Player.Model.Action.Left);

			// Assert
			Assert.IsTrue(result);
		}
		#endregion

		#region ApplyAction
		[TestMethod]
		public void ApplyAction_WithNoAction_StateIsUnchanged()
		{
			// Arrange
			var state = GameSimulator.RandomInitialState();
			var copy = new GameState(state);

			// Act
			state.ApplyAction(Player.Model.Action.NoAction);

			// Assert
			Assert.AreEqual(copy, state);
		}

		[TestMethod]
		public void ApplyAction_ActionIsUpAndIsLegal_UpdatesStateCorrectly()
		{
			// Arrange
			var state = new GameState();

			// - 2 - 2
			// - 2 - -
			// - - - -
			// - - - -
			state.AddTile(new Tile(0, 1, 2));
			state.AddTile(new Tile(0, 3, 2));
			state.AddTile(new Tile(1, 1, 2));

			// - 4 - 2
			// - - - -
			// - - - -
			// - - - -
			var expectedState = new GameState();
			expectedState.AddTile(new Tile(0, 1, 4));
			expectedState.AddTile(new Tile(0, 3, 2));

			// Act
			state.ApplyAction(Player.Model.Action.Up);

			// Assert
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void ApplyAction_ActionIsDownAndIsLegal_UpdatesStateCorrectly()
		{
			// Arrange
			var state = new GameState();

			// - - 4 -
			// - - - -
			// - - - -
			// 4 - 2 -
			state.AddTile(new Tile(0, 2, 4));
			state.AddTile(new Tile(3, 0, 4));
			state.AddTile(new Tile(3, 2, 2));

			// - - - -
			// - - - -
			// - - 4 -
			// 4 - 2 -
			var expectedState = new GameState();
			expectedState.AddTile(new Tile(2, 2, 4));
			expectedState.AddTile(new Tile(3, 0, 4));
			expectedState.AddTile(new Tile(3, 2, 2));

			// Act
			state.ApplyAction(Player.Model.Action.Down);

			// Assert
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void ApplyAction_ActionIsRightAndIsLegal_UpdatesStateCorrectly()
		{
			// Arrange
			var state = new GameState();

			// - - - -
			// - 4 - 4
			// - - 2 -
			// - - - -
			state.AddTile(new Tile(1, 1, 4));
			state.AddTile(new Tile(1, 3, 4));
			state.AddTile(new Tile(2, 2, 2));

			// - - - -
			// - - - 8
			// - - - 2
			// - - - -
			var expectedState = new GameState();
			expectedState.AddTile(new Tile(1, 3, 8));
			expectedState.AddTile(new Tile(2, 3, 2));

			// Act
			state.ApplyAction(Player.Model.Action.Right);

			// Assert
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void ApplyAction_ActionIsLeftAndIsLegal_UpdatesStateCorrectly()
		{
			// Arrange
			var state = new GameState();

			// 2 4 4 -
			// - - - -
			// - - - 2
			// - - - -
			state.AddTile(new Tile(0, 0, 2));
			state.AddTile(new Tile(0, 1, 4));
			state.AddTile(new Tile(0, 2, 4));
			state.AddTile(new Tile(2, 3, 2));

			// 2 8 - -
			// - - - -
			// 2 - - -
			// - - - -
			var expectedState = new GameState();
			expectedState.AddTile(new Tile(0, 0, 2));
			expectedState.AddTile(new Tile(0, 1, 8));
			expectedState.AddTile(new Tile(2, 0, 2));

			// Act
			state.ApplyAction(Player.Model.Action.Left);

			// Assert
			Assert.AreEqual(expectedState, state);
		}
		#endregion
	}
}