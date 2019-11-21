using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Player.Model;

namespace Player.View
{
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	[System.Runtime.InteropServices.ComVisibleAttribute(true)]
	public partial class InteractiveGuideForm : Form
	{
		public const int TimerDelayMs = 100;

		public enum Mode
		{
			Play, Guide, Simulate, Quit
		}

		private GameState State;
		private IGamePlayer Player;
		private GameSimulator Simulator;
		private Mode _Mode;
		private Button[,] GridButtons;
		private Button[] ControlButtons;
		private Label[] ActionLabels;
		private int TurnsTaken = 0;

		public InteractiveGuideForm()
		{
			InitializeComponent();

			Player = new ExpectimaxPlayer();

			GridButtons = new Button[,]
			{
				{ button4, button5, button6, button7 },
				{ button8, button9, button10, button11 },
				{ button12, button13, button14, button15 },
				{ button16, button17, button18, button19 }
			};

			ControlButtons = new Button[]
			{
				button1, button2, button3
			};

			ActionLabels = new Label[]
			{
				label1, label2, label3, label4
			};

			ChangeMode(Mode.Play);
			StartNewGame();
		}

		private void ChangeMode(Mode newMode)
		{
			_Mode = newMode;
			switch (_Mode)
			{
				case Mode.Play:
					foreach (Button b in GridButtons)
					{
						b.Enabled = false;
					}
					foreach (Button b in ControlButtons)
					{
						b.Enabled = true;
					}
					textBox1.Text = "In Play mode, you can play the 2048 game just like you're used to.";
					break;
				case Mode.Guide:
					foreach (Button b in GridButtons)
					{
						b.Enabled = true;
					}
					foreach (Button b in ControlButtons)
					{
						b.Enabled = true;
					}
					textBox1.Text = "In Guide mode, you can use this application alongside the original ";
					textBox1.Text += "game. After each move, click the empty cell in which the new tile ";
					textBox1.Text += "has appeared and enter the number. Use the \"Get Help\" button to ";
					textBox1.Text += "get guidance on the best move!";
					break;
				case Mode.Simulate:
					foreach (Button b in GridButtons)
					{
						b.Enabled = false;
					}
					foreach (Button b in ControlButtons)
					{
						b.Enabled = false;
					}
					textBox1.Text = "In Simulate mode, you can watch the computer play the game!";
					break;
			}
		}

		private void StartNewGame()
		{
			State = GameSimulator.RandomInitialState();
			TurnsTaken = 0;
			UpdateDisplay();
			ClearLabels();
		}

		private void ClearGrid()
		{
			State = new GameState();
			TurnsTaken = 0;
			UpdateDisplay();
			ClearLabels();
		}

		private void UpdateDisplay()
		{
			for (int row = 0; row < GameState.GRID_SIZE; ++row)
			{
				for (int column = 0; column < GameState.GRID_SIZE; ++column)
				{
					if (State.IsCellEmpty(row, column))
					{
						GridButtons[row, column].Text = string.Empty;
						GridButtons[row, column].BackColor = Color.LightGray;
					}
					else
					{
						GridButtons[row, column].Text = State[row, column].ToString();
						GridButtons[row, column].BackColor = Color.Orange;
					}
				}
			}

			label5.Text = $"Moves:   {TurnsTaken}";
		}

		private void ClearLabels()
		{
			foreach (Label label in ActionLabels)
			{
				label.Text = string.Empty;
			}
		}

		private void DoAction(Model.Action action)
		{
			bool wasActionTaken = false;
			if (_Mode == Mode.Play)
				wasActionTaken = GameSimulator.TakeAction(State, action);
			else if (_Mode == Mode.Guide && State.IsActionLegal(action))
				State.ApplyAction(action);

			if (wasActionTaken)
			{
				++TurnsTaken;
				UpdateDisplay();
				DisplayEndOfGameIfNeeded();
			}
		}

		private void DisplayEndOfGameIfNeeded()
		{
			if (State.IsWin)
			{
				PopupDialog winPopup = new PopupDialog(
					"WIN!!!",
					"Congratulations, you won!!",
					this.Location,
					false);
				winPopup.ShowDialog(this);
			}
			else if (State.IsLoss)
			{
				PopupDialog lossPopup = new PopupDialog(
					"GAME OVER",
					"Sorry, try again.",
					this.Location,
					false);
				lossPopup.ShowDialog(this);
			}
		}

		private void GridButtonPressed(int row, int column)
		{
			if (State.IsCellEmpty(row, column))
			{
				PopupDialog popup = new PopupDialog(
					"Edit grid",
					"Enter the number that should go in the button you pressed:",
					this.Location,
					true);
				popup.ShowDialog(this);

				if (Int32.TryParse(popup.UserInput, out int num) && num > 0)
				{
					State.AddTile(new Tile(row, column, num));
					GridButtons[row, column].Text = num.ToString();
					GridButtons[row, column].BackColor = Color.Orange;
				}
			}
		}

		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			if (_Mode == Mode.Play || _Mode == Mode.Guide)
			{
				switch (e.KeyCode)
				{
					case Keys.Up:
						DoAction(Model.Action.Up);
						break;
					case Keys.Down:
						DoAction(Model.Action.Down);
						break;
					case Keys.Right:
						DoAction(Model.Action.Right);
						break;
					case Keys.Left:
						DoAction(Model.Action.Left);
						break;
				}
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			return keyData == Keys.Up || keyData == Keys.Down || 
				keyData == Keys.Left || keyData == Keys.Right;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			_Mode = Mode.Quit;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			StartNewGame();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			ClearGrid();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			pictureBox1.Visible = true;
			ClearLabels();

			var async = new Task<List<ActionValue>>(
				() => new List<ActionValue>(Player.GetPolicies(State)));

			async.ContinueWith((parent) =>
			{
				Invoke(new EventHandler(delegate { pictureBox1.Visible = false; }));

				var actions = parent.Result;
				if (actions.Count == 0)
				{
					Invoke(new EventHandler(delegate
					{
						ActionLabels[0].Text = "NO ACTION";
					}));
				}
				else
				{
					actions.Sort((a, b) => -a.Value.CompareTo(b.Value));

					for (int i = 0; i < actions.Count; ++i)
					{
						var av = actions[i];
						Invoke(new EventHandler(delegate
						{
							ActionLabels[i].Text = $"{av.Action}   {av.Value}";
						}));
					}
				}
			});

			async.Start();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			GridButtonPressed(0, 0);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			GridButtonPressed(0, 1);
		}

		private void button6_Click(object sender, EventArgs e)
		{
			GridButtonPressed(0, 2);
		}

		private void button7_Click(object sender, EventArgs e)
		{
			GridButtonPressed(0, 3);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			GridButtonPressed(1, 0);
		}

		private void button9_Click(object sender, EventArgs e)
		{
			GridButtonPressed(1, 1);
		}

		private void button10_Click(object sender, EventArgs e)
		{
			GridButtonPressed(1, 2);
		}

		private void button11_Click(object sender, EventArgs e)
		{
			GridButtonPressed(1, 3);
		}

		private void button12_Click(object sender, EventArgs e)
		{
			GridButtonPressed(2, 0);
		}

		private void button13_Click(object sender, EventArgs e)
		{
			GridButtonPressed(2, 1);
		}

		private void button14_Click(object sender, EventArgs e)
		{
			GridButtonPressed(2, 2);
		}

		private void button15_Click(object sender, EventArgs e)
		{
			GridButtonPressed(2, 3);
		}

		private void button16_Click(object sender, EventArgs e)
		{
			GridButtonPressed(3, 0);
		}

		private void button17_Click(object sender, EventArgs e)
		{
			GridButtonPressed(3, 1);
		}

		private void button18_Click(object sender, EventArgs e)
		{
			GridButtonPressed(3, 2);
		}

		private void button19_Click(object sender, EventArgs e)
		{
			GridButtonPressed(3, 3);
		}

		// Play mode
		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				ChangeMode(Mode.Play);
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				ChangeMode(Mode.Guide);
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
			{
				ChangeMode(Mode.Simulate);
				ClearLabels();

				Simulator = new GameSimulator(1, State, Player);
				Simulator.ActionTaken += SimulatorTookAction;
				Simulator.GameEnded += SimulatedGameEnded;

				Task.Run(() =>
				{
					Simulator.Run(() => _Mode != Mode.Simulate);
				});
			}
			else
			{
				Simulator.ActionTaken -= SimulatorTookAction;
				Simulator.GameEnded -= SimulatedGameEnded;
			}
		}

		private void SimulatorTookAction(Model.Action action, GameState resultState)
		{
			Invoke(new EventHandler(delegate
			{
				State = resultState;
				label1.Text = action.ToString();
				++TurnsTaken;
				UpdateDisplay();
			}));
		}

		private void SimulatedGameEnded(GameStats result)
		{
			Invoke(new EventHandler(delegate
			{
				DisplayEndOfGameIfNeeded();
			}));
		}
	}
}
