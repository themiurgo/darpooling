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
    public partial class NewTripDialog : Form
    {
        UserNodeCore.UserNodeCore core;

        public NewTripDialog(UserNodeCore.UserNodeCore core)
        {
            this.core = core;
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Communication.Trip trip = new Communication.Trip();
            trip.Owner = core.UserNode.User.UserName; // FIXME?
            trip.ArrivalName = arrivalNameTextBox.Text;
            trip.DepartureName = departureNameTextBox.Text;
            DateTime depDate = departureDatePicker.Value;
            DateTime depTime = departureTimePicker.Value;
            trip.DepartureDateTime = new DateTime(depDate.Year, depDate.Month,
                depDate.Day, depTime.Hour, depTime.Minute, 0);

            DateTime arrDate = arrivalDatePicker.Value;
            DateTime arrTime = arrivalTimePicker.Value;
            trip.ArrivalDateTime = new DateTime(arrDate.Year, arrDate.Month,
                arrDate.Day, arrDate.Hour, arrDate.Minute, 0);
            trip.FreeSits = (int) freeSeatsUpDown.Value;
            trip.Smoke = smokingCheckBox.Checked;
            trip.Music = musicCheckBox.Checked;
            trip.Cost = System.Convert.ToInt32(contributeBox.Text);

            core.InsertTrip(trip);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
