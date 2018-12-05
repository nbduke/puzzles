namespace Player.Model
{
	/// <summary>
	/// The four possible actions in the 2048 game, plus one representing a non-action.
	/// </summary>
	public enum Action
	{
		Up = 0,
		Down,
		Left,
		Right,
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
}