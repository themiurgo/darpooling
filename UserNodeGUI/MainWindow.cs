using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UserNodeGUI
{
    public partial class MainWindow : Form
    {
        UserNodeCore.UserNodeCore core;

        public MainWindow()
        {
            InitializeComponent();
            core = new UserNodeCore.UserNodeCore(new Communication.UserNode("prova"));
            SearchPanel.Hide();
            ResultTabControl.Hide();
        }
                
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new ConnectDialog(core)).ShowDialog();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ResultTabControl.Controls.Add(new TabPage());
        }
    }
}