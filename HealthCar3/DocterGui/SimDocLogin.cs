using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoctorGui
{
    public partial class SimDocLogin : Form
    {
        private string username = "Keesbanaan";
        private string password = "1234";

        public SimDocLogin()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (UsernameBox.Text.Equals(username) && textBox2.Text.Equals(password))
            {
                Hide();
                SimDocHome simdochome = new SimDocHome(UsernameBox.Text);
                simdochome.ShowDialog();
            }
            else if (string.IsNullOrEmpty(UsernameBox.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("You must enter a username and password");
            }
            else if (!UsernameBox.Text.Equals(username) || !textBox2.Text.Equals(password))
            {
                MessageBox.Show("Wrong username or password");
            }
        }
    }
}

