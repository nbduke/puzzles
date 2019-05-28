/*
 * ShowPathsForm.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains a partial definition of the ShowPathsForm class dealing with user 
 * event handling.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Tools.DataStructures;
using Wordament.Model;

namespace Wordament.View
{
	/*
	 * ShowPathsForm animates the path of tiles on the grid for each word returned by the finder
	 * algorithm. The grid is replicated as closely as possible. The "Show next path" button
	 * allows the user to cycle through paths at his or her own pace.
	 */
	public partial class ShowPathsForm : Form
	{
		private List<WordamentPath> AllPaths { get; set; }
		private int NextPathIdx { get; set; }
		private Grid<PictureBox> TilePics { get; set; }

		private const int ANIMATION_DELAY_MS = 100;

		public static Size TILE_IMAGE_SIZE = new Size(80, 80);
		public static Size BACKGROUND_IMAGE_SIZE = new Size(356, 356);

		public ShowPathsForm(List<WordamentPath> allPaths, int roundNum)
		{
			InitializeComponent();

			AllPaths = allPaths;
			NextPathIdx = 0;

			TilePics = Grid<PictureBox>.Unflatten(new PictureBox[]
				{
					pictureBox2, pictureBox3, pictureBox4, pictureBox5,
					pictureBox9, pictureBox8, pictureBox7, pictureBox6,
					pictureBox13, pictureBox12, pictureBox11, pictureBox10,
					pictureBox17, pictureBox16, pictureBox15, pictureBox14
				},
				SetupGridForm.GridRows,
				SetupGridForm.GridColumns
			);

			this.Text = string.Format("Paths - Round {0}", roundNum);
			pictureBox1.Image = TileImageTable.GetImage(ImageType.Background);
		}

		/*
		 * Replaces all tile images in the previously displayed path with the "not in path" image. 
		 * NOTE: This method should not be run in a synchronous event handler; it is meant to be called 
		 * within a concurrent thread. 
		 */
		private void EraseLastPath()
		{
			if (NextPathIdx == 0) // if the first path has yet to be displayed, we must set every tile's image
			{
				foreach (PictureBox pic in TilePics)
				{
					pic.Invoke(new EventHandler(delegate { pic.Image = TileImageTable.GetImage(ImageType.NotInPath); }));
				}
			}
			else
			{
				foreach (GridCell position in AllPaths[NextPathIdx - 1].Locations)
				{
					PictureBox pic = TilePics[position];
					pic.Invoke(new EventHandler(delegate { pic.Image = TileImageTable.GetImage(ImageType.NotInPath); }));
				}
			}
		}

		/*
		 * Animate the next path. NOTE: This method should not be run in a synchronous event handler; it is 
		 * meant to be called within a concurrent thread. 
		 */
		private void VisualizePath()
		{
			// If there are no more paths to display
			if (NextPathIdx == AllPaths.Count)
			{
				PopupDialog popup = new PopupDialog(
					"No more words!",
					"You have entered all the words we've found on the grid.",
					this.Location,
					false);
				popup.ShowDialog();
				return;
			}

			WordamentPath nextPath = AllPaths[NextPathIdx++];
			WordLabel.Invoke(new EventHandler(delegate { WordLabel.Text = nextPath.Word; }));
			ScoreLabel.Invoke(new EventHandler(delegate { ScoreLabel.Text = nextPath.TotalScore.ToString(); }));
			CounterLabel.Invoke(new EventHandler(delegate { CounterLabel.Text = $"{NextPathIdx}/{AllPaths.Count}"; }));

			if (nextPath.Locations.Count == 1)
			{
				// If the path is one tile long, just put a stop image on it
				GridCell startingPt = nextPath.Locations[0];
				PictureBox pic = TilePics[startingPt];
				pic.Invoke(new EventHandler(delegate { pic.Image = TileImageTable.GetImage(ImageType.Stop); }));
			}
			else
			{
				// Select the starting tile image
				GridCell startingPt = nextPath.Locations[0];
				PictureBox startPic = TilePics[startingPt];
				startPic.Invoke(new EventHandler(delegate { startPic.Image = GetStartImage(startingPt, nextPath.Locations[1]); }));

				Thread.Sleep(ANIMATION_DELAY_MS);

				// Select each of the inner tile images
				for (int i = 1; i < nextPath.Locations.Count - 1; i++)
				{
					GridCell currentPt = nextPath.Locations[i];
					PictureBox pic = TilePics[currentPt];
					pic.Invoke(new EventHandler(delegate { pic.Image = GetPathImage(currentPt, nextPath.Locations[i + 1]); }));
					Thread.Sleep(ANIMATION_DELAY_MS);
				}

				// Put a stop image on the final tile
				GridCell lastPt = nextPath.Locations[nextPath.Locations.Count - 1];
				PictureBox lastPic = TilePics[lastPt];
				lastPic.Invoke(new EventHandler(delegate { lastPic.Image = TileImageTable.GetImage(ImageType.Stop); }));
			}
		}

		/*
		 * Returns the image for the first tile on the current path. The image is of an arrow pointing from the 
		 * current tile to the next one, so two coordinate locations must be given in order to select the correct 
		 * arrow.
		 */
		private Image GetStartImage(GridCell currentPt, GridCell nextPt)
		{
			int rowDiff = nextPt.Row - currentPt.Row;
			int columnDiff = nextPt.Column -currentPt.Column;

			if (rowDiff > 0)
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.Start_Southeast);
				else if (columnDiff < 0)
					return TileImageTable.GetImage(ImageType.Start_Southwest);
				else
					return TileImageTable.GetImage(ImageType.Start_South);
			}
			else if (rowDiff < 0)
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.Start_Northeast);
				else if (columnDiff < 0)
					return TileImageTable.GetImage(ImageType.Start_Northwest);
				else
					return TileImageTable.GetImage(ImageType.Start_North);
			}
			else
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.Start_East);
				else
					return TileImageTable.GetImage(ImageType.Start_West);
			}
		}

		/*
		 * Returns the image for the current tile on the current path (use GetStartingImage if the tile is the
		 * first one on a path). 
		 */
		private Image GetPathImage(GridCell currentPt, GridCell nextPt)
		{
			int rowDiff = nextPt.Row - currentPt.Row;
			int columnDiff = nextPt.Column - currentPt.Column;

			if (rowDiff > 0)
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.Southeast);
				else if (columnDiff < 0)
					return TileImageTable.GetImage(ImageType.Southwest);
				else
					return TileImageTable.GetImage(ImageType.South);
			}
			else if (rowDiff < 0)
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.Northeast);
				else if (columnDiff < 0)
					return TileImageTable.GetImage(ImageType.Northwest);
				else
					return TileImageTable.GetImage(ImageType.North);
			}
			else
			{
				if (columnDiff > 0)
					return TileImageTable.GetImage(ImageType.East);
				else
					return TileImageTable.GetImage(ImageType.West);
			}
		}

		// Called when the form is displayed for the first time
		private void ShowPathsForm_Shown(object sender, EventArgs e)
		{
			Task.Run(() =>
				{
					EraseLastPath();
					VisualizePath();
				}
			);
		}

		// Next
		private void button1_Click(object sender, EventArgs e)
		{
			Task.Run(() =>
				{
					EraseLastPath();
					VisualizePath();
				}
			);
		}

		// End round
		private void button2_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
