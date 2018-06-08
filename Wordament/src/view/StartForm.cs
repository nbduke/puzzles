/*
 * StartForm.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains a partial definition of the StartForm class dealing with user 
 * event handling.
 */

using System;
using System.Windows.Forms;

namespace Wordament.View
{
	/*
	 * StartForm is a simple intro page for the app. It has an image and a Start button.
	 */
	public partial class StartForm : Form
	{
		public StartForm()
		{
			InitializeComponent();

			// TODO add an image to the form
		}

		// Start
		private void button1_Click(object sender, EventArgs e)
		{
			// Launch the SetupGridForm
			SetupGridForm setupForm = new SetupGridForm();
			setupForm.StartPosition = FormStartPosition.Manual;
			setupForm.Location = this.Location;
			setupForm.FormClosed += new FormClosedEventHandler(
				(object obj, FormClosedEventArgs args) => { this.Show(); });

			this.Hide();
			setupForm.Show();
		}
	}
}
