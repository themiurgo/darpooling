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
            core.resultCallback += new UserNodeCore.UserNodeCore.ResultReceiveHandler(onNewResult);
        }

        /// <summary>
        /// Each new Result received (through callback in NodeCore), goes
        /// here, then is processed in an appropriate method, exploiting
        /// overloading to change behaviour depending on Result subclass.
        /// </summary>
        /// <param name="result"></param>
        public void onNewResult(Communication.Result result) {
            
        }

        public void ProcessResult(Communication.LoginOkResult result)
        {
        }

        public void ProcessResult(Communication.LoginErrorResult result)
        {
        }
                
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog dlg = new ConnectDialog(core);           
            dlg.SetConnectedViewCallback = new ConnectDialog.SetConnectedViewDelegate(this.SetConnectedView);
            dlg.ShowDialog();

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            ResultTabControl.Controls.Add(new TabPage());
        }

        private void SetConnectedView(bool connected)
        {
            if (connected)
            {
                connectedStatusLabel.Text = "Connected";
                connectToolStripMenuItem.Enabled = false;
            }
            else
            {
                connectedStatusLabel.Text = "Not connected";
                connectToolStripMenuItem.Enabled = true;
            }
        }
    }
}