/*
 * SetupGridForm.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains a partial definition of the SetupGridForm class dealing with user 
 * event handling.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using Tools.DataStructures;
using Wordament.Model;

namespace Wordament.View
{
	/*
	 * SetupGridForm allows the user to specify the Wordament grid in each round
	 * of the game. The GUI consists of a 4x4 grid of text boxes for entering tile
	 * strings. Next to each text box is a non-editable label displaying the score
	 * for that tile. Score are calculated automatically (using TileScoreTable) but
	 * may be set manually by double-clicking the score label. For speed rounds, all
	 * scores are set to 0. 
	 */
	public partial class SetupGridForm : Form
	{
		public const int GridRows = 4;
		public const int GridColumns = 4;
		public const uint MinimumWordLength = 3;

		private int RoundNum { get; set; }
		private bool IsSpeedRound { get; set; }
		private Grid<TextBox> TileFields { get; set; }
		private Grid<Label> ScoreLabels { get; set; }

		public SetupGridForm()
		{
			InitializeComponent();

			RoundNum = 1;
			IsSpeedRound = false;
			this.Text = string.Format("Setup Grid - Round {0}", RoundNum);

			TileFields = new Grid<TextBox>(GridRows, GridColumns,
				new TextBox[]
				{
					textBox1, textBox2, textBox3, textBox4,
					textBox8, textBox7, textBox6, textBox5,
					textBox12, textBox11, textBox10, textBox9,
					textBox16, textBox15, textBox14, textBox13
				});

			ScoreLabels = new Grid<Label>(GridRows, GridColumns,
				new Label[]
				{
					label1, label2, label3, label4,
					label8, label7, label6, label5,
					label12, label11, label10, label9,
					label16, label15, label14, label13
				});
		}

		// Go
		private void button1_Click(object sender, EventArgs e)
		{
			// Construct a TileGrid from the form
			List<Tile> tiles = new List<Tile>();
			for (int i = 0; i < GridRows; i++)
			{
				for (int j = 0; j < GridColumns; j++)
				{
					string letters = TileFields[i, j].Text;
					string score = ScoreLabels[i, j].Text;

					if (string.IsNullOrEmpty(letters)) // handle an empty tile field
					{
						PopupDialog errMsg = new PopupDialog(
							"Oops!",
							"You must enter a string in every tile field.",
							this.Location,
							false);
						errMsg.ShowDialog(this);

						TileFields[i, j].Focus();
						return;
					}
					else
					{
						try
						{
							tiles.Add(Tile.Parse(letters, score, new GridCell(i, j)));
						}
						catch (InvalidDataException) // handle a tile field that couldn't be parsed
						{
							PopupDialog errMsg = new PopupDialog(
								"Oops!",
								string.Format("The tile specified by \"{0}\", \"{1}\" could not be parsed. Check your formatting.", 
									letters, score),
								this.Location,
								false);
							errMsg.ShowDialog(this);

							TileFields[i, j].Focus();
							return;
						}
					}
				}
			}

			RunWordFinder(new Grid<Tile>(GridRows, GridColumns, tiles));
		}

		/*
		 * Runs the word finder algorithm and launches the ShowPathsForm.
		 */
		private void RunWordFinder(Grid<Tile> grid)
		{
			// Run the word finder algorithm.
			WordFinder finder = new WordFinder(Program.Dictionary, grid);
			List<WordamentPath> foundPaths = finder.FindWordsWithPaths(MinimumWordLength);

			// If it's a speed round, sort by ascending path length. Otherwise, sort by
			// descending total score.
			if (IsSpeedRound)
				foundPaths.Sort((a, b) => a.Word.Length.CompareTo(b.Word.Length));
			else
				foundPaths.Sort((a, b) => -a.TotalScore.CompareTo(b.TotalScore));

			if (foundPaths.Count == 0)
			{
				// Display an error message
				PopupDialog noResultsDialog = new PopupDialog(
					"Error",
					"We could not find any words on the grid. Be sure that you specified each tile correctly.",
					this.Location,
					false);
				noResultsDialog.ShowDialog();
			}
			else
			{
				// Launch the next window
				ShowPathsForm pathsForm = new ShowPathsForm(foundPaths, RoundNum);
				pathsForm.StartPosition = FormStartPosition.Manual;
				pathsForm.Location = this.Location;
				pathsForm.FormClosed += new FormClosedEventHandler(
					(object obj, FormClosedEventArgs args) => { ReceivedFocus(); });

				this.Hide();
				pathsForm.Show();
			}
		}

		/*
		 * Called whenever focus is given to this form (i.e. when the form first loads and when 
		 * ShowPathsForm closes).
		 */
		private void ReceivedFocus()
		{
			// Clear the form
			for (int i = 0; i < GridRows; i++)
			{
				for (int j = 0; j < GridColumns; j++)
				{
					TileFields[i, j].Text = string.Empty;
					ScoreLabels[i, j].Text = string.Empty;
				}
			}

			checkBox1.Checked = false;
			this.Text = string.Format("Setup Grid - Round {0}", ++RoundNum);

			this.Show();
			textBox1.Focus();
		}

		// Speed round
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			IsSpeedRound = !IsSpeedRound;

			// Set tile scores accordingly
			for (int i = 0; i < GridRows; i++)
			{
				for (int j = 0; j < GridColumns; j++)
				{
					AutoSetScore(ScoreLabels[i, j], TileFields[i, j].Text);
				}
			}
		}

		/*
		 * Automatically display the score associated with the given tile string on the given label
		 */
		private void AutoSetScore(Label label, string tileString)
		{
			if (IsSpeedRound)
				label.Text = "0";
			else if (string.IsNullOrEmpty(tileString))
				label.Text = string.Empty;
			else
				label.Text = TileScoreTable.LookupScore(tileString).ToString();
		}

		/*
		 * Issue a PopupDialog allowing the user to enter manually a tile's score
		 */
		private void ManualSetScore(Label label, string tileString)
		{
			// Only permit the manual entry if it's not a speed round and if the tile string has been entered
			if (!IsSpeedRound && !string.IsNullOrEmpty(tileString))
			{
				PopupDialog manualEntry = new PopupDialog(
					"Set score",
					"Enter a number below to set the tile score manually: ",
					this.Location,
					true,
					(string userInput) =>
					{
						if (!string.IsNullOrEmpty(userInput))
							label.Text = userInput.Trim();
					});
				manualEntry.ShowDialog();
			}
		}

		/*
		 * This region contains handlers for the TextBox.Leave event. When the user finishes editing 
		 * a tile field, this handler calls AutoSetScore using the text in the field.
		 */
		#region TextBox.Leave

		private void textBox1_Leave(object sender, EventArgs e)
		{
			textBox1.Text = textBox1.Text.Trim();
			AutoSetScore(label1, textBox1.Text);
		}
		
		private void textBox2_Leave(object sender, EventArgs e)
		{
			textBox2.Text = textBox2.Text.Trim();
			AutoSetScore(label2, textBox2.Text);
		}

		private void textBox3_Leave(object sender, EventArgs e)
		{
			textBox3.Text = textBox3.Text.Trim();
			AutoSetScore(label3, textBox3.Text);
		}

		private void textBox4_Leave(object sender, EventArgs e)
		{
			textBox4.Text = textBox4.Text.Trim();
			AutoSetScore(label4, textBox4.Text);
		}

		private void textBox5_Leave(object sender, EventArgs e)
		{
			textBox5.Text = textBox5.Text.Trim();
			AutoSetScore(label5, textBox5.Text);
		}

		private void textBox6_Leave(object sender, EventArgs e)
		{
			textBox6.Text = textBox6.Text.Trim();
			AutoSetScore(label6, textBox6.Text);
		}

		private void textBox7_Leave(object sender, EventArgs e)
		{
			textBox7.Text = textBox7.Text.Trim();
			AutoSetScore(label7, textBox7.Text);
		}

		private void textBox8_Leave(object sender, EventArgs e)
		{
			textBox8.Text = textBox8.Text.Trim();
			AutoSetScore(label8, textBox8.Text);
		}

		private void textBox9_Leave(object sender, EventArgs e)
		{
			textBox9.Text = textBox9.Text.Trim();
			AutoSetScore(label9, textBox9.Text);
		}

		private void textBox10_Leave(object sender, EventArgs e)
		{
			textBox10.Text = textBox10.Text.Trim();
			AutoSetScore(label10, textBox10.Text);
		}

		private void textBox11_Leave(object sender, EventArgs e)
		{
			textBox11.Text = textBox11.Text.Trim();
			AutoSetScore(label11, textBox11.Text);
		}

		private void textBox12_Leave(object sender, EventArgs e)
		{
			textBox12.Text = textBox12.Text.Trim();
			AutoSetScore(label12, textBox12.Text);
		}

		private void textBox13_Leave(object sender, EventArgs e)
		{
			textBox13.Text = textBox13.Text.Trim();
			AutoSetScore(label13, textBox13.Text);
		}

		private void textBox14_Leave(object sender, EventArgs e)
		{
			textBox14.Text = textBox14.Text.Trim();
			AutoSetScore(label14, textBox14.Text);
		}

		private void textBox15_Leave(object sender, EventArgs e)
		{
			textBox15.Text = textBox15.Text.Trim();
			AutoSetScore(label15, textBox15.Text);
		}

		private void textBox16_Leave(object sender, EventArgs e)
		{
			textBox16.Text = textBox16.Text.Trim();
			AutoSetScore(label16, textBox16.Text);
		}

		#endregion TextBox.Leave

		/*
		 * This region contains handlers for the Label.DoubleClick event. When a tile's score label is 
		 * double-clicked, the user can manually enter that tile's score.
		 */
		#region Label.DoubleClick

		private void label1_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label1, textBox1.Text);
		}

		private void label2_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label2, textBox2.Text);
		}

		private void label3_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label3, textBox3.Text);
		}

		private void label4_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label4, textBox4.Text);
		}

		private void label5_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label5, textBox5.Text);
		}

		private void label6_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label6, textBox6.Text);
		}

		private void label7_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label7, textBox7.Text);
		}

		private void label8_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label8, textBox8.Text);
		}

		private void label9_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label9, textBox9.Text);
		}

		private void label10_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label10, textBox10.Text);
		}

		private void label11_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label11, textBox11.Text);
		}

		private void label12_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label12, textBox12.Text);
		}

		private void label13_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label13, textBox13.Text);
		}

		private void label14_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label14, textBox14.Text);
		}

		private void label15_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label15, textBox15.Text);
		}

		private void label16_DoubleClick(object sender, EventArgs e)
		{
			ManualSetScore(label16, textBox16.Text);
		}

		#endregion Label.DoubleClick

		/* // TODO maybe try this later
		private void ParseSpeedyEntry(string input)
		{
			if (string.IsNullOrEmpty(input))
				return;

			List<Tile> tiles = new List<Tile>();
			int counter = 0;
			try
			{
				if (input.Contains("/s"))
				{
					input = input.Remove(input.IndexOf("/s"), 2);
					string[] parts = input.Split(' ');
					if (parts.Length != TileGrid.GRID_DIM * TileGrid.GRID_DIM)
					{
						PopupDialog errMsg = new PopupDialog(
							"Oops!",
							"You have not entered enough letters to fill the grid.",
							this.Location,
							false);
						errMsg.ShowDialog(this);
						return;
					}
					else
					{
						foreach (string tileStr in parts)
						{
							tiles.Add(Tile.Parse(tileStr, 0,
								new CoordinatePair(counter++ / TileGrid.GRID_DIM, counter % TileGrid.GRID_DIM)));
						}
					}
				}
				else
				{
					string[] parts = input.Split(' ');
					foreach (string tileStr in parts)
					{
						tiles.Add(Tile.Parse(tileStr, TileScoreTable.LookupScore(tileStr).ToString(),
							new CoordinatePair(counter++ / TileGrid.GRID_DIM, counter % TileGrid.GRID_DIM)));
					}
				}
			}
			catch (InvalidDataException)
			{
				PopupDialog errMsg = new PopupDialog(
					"Oops!",
					"We failed to parse the speedy entry.",
					this.Location,
					false);
				errMsg.ShowDialog(this);
				return;
			}

			RunWordFinder(new TileGrid(tiles));
		}

		// Speedy entry
		private void button2_Click(object sender, EventArgs e)
		{
			PopupDialog speedyEntryForm = new PopupDialog(
				"Speedy Entry",
				"Enter all of the tile strings, each separated by a single space, in the text field below. " +
					"Do not include the tile scores. If the round is a speed round, add /s before the " +
					"first or after the last tile.",
				this.Location,
				true,
				ParseSpeedyEntry);
			speedyEntryForm.ShowDialog();
		}
		*/
	}
}
