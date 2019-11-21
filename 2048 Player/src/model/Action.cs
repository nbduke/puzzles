using System;
using System.Collections.Generic;
using Tools.DataStructures;

namespace Player.Model
{
	/// <summary>
	/// The four possible actions in the 2048 game, plus one representing a non-action.
	/// </summary>
	public enum Action
	{
		Left = 0,
		Up,
		Right,
		Down,
		NoAction
	}

	/// <summary>
	/// Represents an action and its expected value.
	/// </summary>
	public struct ActionValue
	{
		public Action Action;
		public double Value;

		public ActionValue(Action action, double value)
		{
			Action = action;
			Value = value;
		}
	}

	public static class ActionValueCollectionExtensions
	{
		public static Action Best(this IEnumerable<ActionValue> actionValues)
		{
			ActionValue best = new ActionValue()
			{
				Action = Action.NoAction,
				Value = double.MinValue
			};

			foreach (ActionValue av in actionValues)
			{
				if (av.Value > best.Value)
					best = av;
			}

			return best.Action;
		}
	}
}