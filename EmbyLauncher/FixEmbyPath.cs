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
    public partial class FixEmbyPath : Form
    {
        public FixEmbyPath()
        {
            InitializeComponent();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("msiexec.exe", "/x {0AE55B2A-F3B7-4297-A950-BEBE7DFC68DF}");
            Application.Exit();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Properties.Settings.Default.EmbyURL);
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.EmbyLocationOverride = openFileDialog1.FileName;
                Properties.Settings.Default.Save();
                //this.Close();
                //ProcessStartInfo Info = new ProcessStartInfo();
                //Info.Arguments = "/C ping 127.0.0.1 -n 2 && \"" + Application.ExecutablePath + "\"";
                //Info.WindowStyle = ProcessWindowStyle.Hidden;
                //Info.CreateNoWindow = true;
                //Info.FileName = "cmd.exe";
                //Process.Start(Info);
                //Application.Exit(); 
                Process.Start(Application.ExecutablePath);
                Application.Exit();
            }
            else
            {
            }
        }
    }
}
