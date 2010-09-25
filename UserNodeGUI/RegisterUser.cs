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
    public partial class RegisterUser : Form
    {
        private UserNodeCore.UserNodeCore core;

        public RegisterUser(UserNodeCore.UserNodeCore core)
        {
            this.core = core;
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            Communication.User.Sex sex;
            if (maleRadioButton.Checked)
                sex = Communication.User.Sex.m;
            else if (femaleRadioButton.Checked)
                sex = Communication.User.Sex.f;
            else
                return;

            Communication.User user = new Communication.User()
            {
                UserName = usernameTextBox.Text,
                Password = passwordTextBox.Text,
                Name = nameTextBox.Text,
                Email = emailTextBox.Text,
                BirthDate = birthdateTimePicker.Value,
                UserSex = sex,
                Whereabouts = locationTextBox.Text,
                Smoker = smokerCheckBox.Checked,
                SignupDate = DateTime.Now
            };
            // Forward to core (TODO)
        }
    }
}
