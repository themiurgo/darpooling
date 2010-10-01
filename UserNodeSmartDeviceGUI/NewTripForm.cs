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
    public partial class NewTripForm : Form
    {
        private SearchForm main;

        public SearchForm Main
        {
            get { return main; }
            set { main = value; }
        }

        public NewTripForm()
        {
            InitializeComponent();
            main = null;
        }

        private void cancelMenuItem_Click(object sender, EventArgs e)
        {
            Dispose();
            if (main != null)
                main.Show();
        }
    }
}