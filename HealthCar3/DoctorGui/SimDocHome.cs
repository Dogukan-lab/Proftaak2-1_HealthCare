using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DoctorGui
{
    public partial class SimDocHome : Form
    {
        public SimDocHome(string username)
        {
            InitializeComponent();
            DoctorWelcome.Text = "Welcome Docter " + username;
        }

        private void Docter_Click(object sender, EventArgs e)
        {

        }

        private void SimDocHome_Load(object sender, EventArgs e)
        {

        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }
    }
}
