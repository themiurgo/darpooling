using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserNodeSmartDeviceGUI
{
    public partial class MainForm : Form
    {
        private delegate void VoidDelegate();

        public MainForm()
        {
            InitializeComponent();
        }

        public void Connect(string username, string password, string serviceAddress)
        {

        }

        private void ConnectForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Up))
            {
                // Up
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                // Down
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                // Left
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                // Right
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                // Enter
            }

        }

        private void registerItem_Click(object sender, EventArgs e)
        {
        }

        private void connectItem_Click(object sender, EventArgs e)
        {
            if (usernameTextBox.Text.Length == 0 ||
                passwordTextBox.Text.Length == 0 ||
                serverTextBox.Text.Length == 0)
                MessageBox.Show("Please, fill all fields");
            

            SearchForm sf = new SearchForm(this);
            sf.Show();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            usernameTextBox.Text = "";
            passwordTextBox.Text = "";
            serverTextBox.Text = "";
        }
    }
}