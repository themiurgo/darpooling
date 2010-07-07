using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientGUI
{
    public partial class MainWindow : Form
    {
        ConnectDialog connectDialog = new ConnectDialog();

        public MainWindow()
        {
            InitializeComponent();
        }
                
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connectDialog.ShowDialog();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            searchTabControl.Controls.Add(new TabPage());
        }  
    }
}