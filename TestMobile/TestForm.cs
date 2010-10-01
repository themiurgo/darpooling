using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TestMobile
{
    public partial class TestForm : Form
    {
        private string requestID;
        DarPoolingMobileClient serviceProxy = new DarPoolingMobileClient();

        public TestForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            
            JoinCommand join = new JoinCommand();
            join.UserName = "daniele@http://localhost:1111/Milano";
            join.PasswordHash = Communication.Tools.HashString("dani");
            try
            {
                requestID = serviceProxy.HandleDarPoolingMobileRequest(join);
                //MessageBox.Show(response);
                if (requestID != null)
                    connectionLabel.Text = "Request sent";
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void resultButton_Click(object sender, EventArgs e)
        {
            //Command c = new Command();
           

            Result result;
            try
            {
                result = serviceProxy.GetMobileResult(requestID);
                //requestID = serviceProxy.HandleDarPoolingMobileRequest(join);
                //MessageBox.Show(response);
                if (result != null)
                    resultLabel.Text = "Got a: " + result.GetType().Name;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}