using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace EmbyLauncher
{
    public partial class IncreaseWait : Form
    {
        public IncreaseWait()
        {
            InitializeComponent();
            this.numericUpDown1.Value = Properties.Settings.Default.WaitForNetwork;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Application.ExecutablePath);
            Application.Exit();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Properties.Settings.Default.WaitForNetwork = Convert.ToInt32(numericUpDown1.Value);
            Properties.Settings.Default.Save();
            Process.Start(Application.ExecutablePath);
            Application.Exit();
        }
    }
}
