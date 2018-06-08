/*
 * PopupDialog.cs
 * 
 * Nathan Duke
 * 7/29/13
 * WordamentApp
 * 
 * Contains a partial definition of the PopupDialog class dealing with user 
 * event handling.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wordament.View
{
	public delegate void UserAccepted(string userInput);
	public delegate void UserCanceled(string userInput);

	/*
	 * PopupDialog implements a simple customizable popup (i.e. modal) dialog box. An instance is 
	 * constructed with a title to display at the top of the window, a message for the window's main
	 * panel, the screen coordinate location at which to display the window, and a boolean specifying
	 * whether an input text box should be displayed. In addition, the delegates UserAccepted and 
	 * UserCanceled can be supplied and will be called, respecitively, when the "Ok" or the "Cancel" 
	 * button is pressed. 
	 */
	public partial class PopupDialog : Form
	{
		private UserAccepted AcceptAction { get; set; }
		private UserCanceled CancelAction { get; set; }

		public PopupDialog(
			string title, 
			string message,
			Point startingPoint,
			bool getUserInput, 
			UserAccepted acceptAction = null, 
			UserCanceled cancelAction = null)
		{
			this.StartPosition = FormStartPosition.Manual;
			this.Location = startingPoint;

			InitializeComponent();

			AcceptAction = acceptAction;
			CancelAction = cancelAction;

			this.Text = title;
			label1.Text = message;

			if (getUserInput)
			{
				textBox1.Show();
				textBox1.Focus();
			}
		}

		// Ok 
		private void button1_Click(object sender, EventArgs e)
		{
			if (AcceptAction != null)
				AcceptAction(textBox1.Text);

			this.Close();
		}

		// Cancel
		private void button2_Click(object sender, EventArgs e)
		{
			if (CancelAction != null)
				CancelAction(textBox1.Text);

			this.Close();
		}

		// On return from textBox1, simulate a click to the Ok button
		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
				button1_Click(this, new EventArgs());
		}
	}
}
