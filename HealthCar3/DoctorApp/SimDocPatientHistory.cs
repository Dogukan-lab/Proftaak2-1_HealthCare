using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DoctorGui
{
    public partial class SimDocPatientHistory : Form
    {
        public SimDocPatientHistory()
        {
            InitializeComponent();
            DoctorWelcome.Text = "Welcome Docter ";
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

        private void BikePatientButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SimDocBikePatient simDocBikePatient = new SimDocBikePatient();
            simDocBikePatient.ShowDialog();
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SimDocHome simDocHome = new SimDocHome("Welcome");
            simDocHome.ShowDialog();

        }
    }
}
