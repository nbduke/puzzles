using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Player.Model;

namespace Test {

	[TestClass]
	public class GameStateUnitTests
	{
		#region IsActionLegal
		[TestMethod]
		public void IsActionLegal_WithNoAction_ReturnsFalse()
		{
			// Arrange
			var state = GameState.RandomInitialState();

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

		#region DoAction
		[TestMethod]
		public void DoAction_WithNoAction_StateIsUnchangedAndReturnsFalse()
		{
			// Arrange
			var state = GameState.RandomInitialState();
			var copy = new GameState(state);

			// Act
			bool result = state.DoAction(Player.Model.Action.NoAction);

			// Assert
			Assert.IsFalse(result);
			Assert.AreEqual(copy, state);
		}

		[TestMethod]
		public void DoAction_ActionIsUpAndIsLegal_UpdatesStateCorrectlyAndReturnsTrue()
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
			var result = state.DoAction(Player.Model.Action.Up);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void DoAction_ActionIsDownAndIsLegal_UpdatesStateCorrectlyAndReturnsTrue()
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
			bool result = state.DoAction(Player.Model.Action.Down);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void DoAction_ActionIsRightAndIsLegal_UpdatesStateCorrectlyAndReturnsTrue()
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
			bool result = state.DoAction(Player.Model.Action.Right);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(expectedState, state);
		}

		[TestMethod]
		public void DoAction_ActionIsLeftAndIsLegal_UpdatesStateCorrectlyAndReturnsTrue()
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
			bool result = state.DoAction(Player.Model.Action.Left);

			// Assert
			Assert.IsTrue(result);
			Assert.AreEqual(expectedState, state);
		}
		#endregion
	}

}