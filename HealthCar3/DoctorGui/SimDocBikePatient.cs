﻿using System;
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

        private void HomeButton_Click(object sender, EventArgs e)
        {
            //            this.Hide();
            //<<<<<<< Updated upstream:HealthCar3/DoctorGui/SimDocBikePatient.cs
            //            SimDocHome simDocHome = new SimDocHome("Welcome");
            //=======
            //            SimDocHome simDocHome = new SimDocHome("Place holder");
            //>>>>>>> Stashed changes:HealthCar3/DocterGui/SimDocBikePatient.cs
            //            simDocHome.ShowDialog();
        }

        private void HomeButton_Click_1(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}