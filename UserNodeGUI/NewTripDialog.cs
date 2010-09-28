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
            DateTime departure = DateTime.Now.AddDays(1);
            DateTime arrival = departure.AddHours(2);
            departureDatePicker.Value = departure;
            departureTimePicker.Value = departure;
            arrivalDatePicker.Value = arrival;
            arrivalTimePicker.Value = arrival;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            DateTime depDate = departureDatePicker.Value;
            DateTime depTime = departureTimePicker.Value;
            DateTime arrDate = arrivalDatePicker.Value;
            DateTime arrTime = arrivalTimePicker.Value;

            Communication.Trip trip = new Communication.Trip()
            {
                Owner = core.UserNode.User.UserName, // FIXME?
                DepartureName = departureNameTextBox.Text,
                DepartureDateTime = new DateTime(depDate.Year, depDate.Month,
                    depDate.Day, depTime.Hour, depTime.Minute, 0),
                ArrivalName = arrivalNameTextBox.Text,
                ArrivalDateTime = new DateTime(arrDate.Year, arrDate.Month,
                    arrDate.Day, arrDate.Hour, arrDate.Minute, 0),
                FreeSits = (int) freeSeatsUpDown.Value,
                Smoke = smokingCheckBox.Checked,
                Music = musicCheckBox.Checked,
                Cost = (int) contributeUpDown.Value,
                Notes = notesTextBox.Text
            };

            core.InsertTrip(trip);
            Dispose();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
