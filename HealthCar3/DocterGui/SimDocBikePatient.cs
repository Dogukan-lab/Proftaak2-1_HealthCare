using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DoctorGui
{
    public partial class SimDocBikePatient : Form
    {
        public SimDocBikePatient()
        {
            InitializeComponent();
            
        }

        private void Docter_Click(object sender, EventArgs e)
        {

        }

        private void SimDocHome_Load(object sender, EventArgs e)
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            this.Location = new Point(0,0);
            this.Size = new Size(w, h);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }

        private void PatientHistoryButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SimDocPatientHistory simDocPatientHistory = new SimDocPatientHistory();
            simDocPatientHistory.ShowDialog();
        }
    }
}
