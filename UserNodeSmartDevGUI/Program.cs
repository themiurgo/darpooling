using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UserNodeSmartDevGUI
{
    static class Program
    {
        /// <summary>
        /// Il punto di ingresso principale dell'applicazione.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Application.Run(new LoginForm());
        }
    }
}