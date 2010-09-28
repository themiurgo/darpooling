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
    public partial class ConnectDialog : Form
    {
        private UserNodeCore.UserNodeCore core;
        public delegate void SetConnectedViewDelegate(bool status); 
        public SetConnectedViewDelegate SetConnectedViewCallback;

        public ConnectDialog()
        {
            InitializeComponent();
        }

        public ConnectDialog(UserNodeCore.UserNodeCore core)
        {
            InitializeComponent();
            this.core = core;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void ConnectDialog_Load(object sender, EventArgs e)
        {
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                core.Join(UsernameTextbox.Text, PasswordTextBox.Text,
                    AddressComboBox.Text, "http://localhost:2222");
            }
            catch
            {
                MessageBox.Show("Connection error");
            }
        }
    }
}