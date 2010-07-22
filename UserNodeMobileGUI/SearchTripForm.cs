using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UserNodeMobileGUI
{
    public partial class SearchTripForm : DarpoolingForm
    {
        public SearchTripForm(UserNodeCore.UserNodeCore core) :
            base(core)
        {
            InitializeComponent();
        }
    }
}