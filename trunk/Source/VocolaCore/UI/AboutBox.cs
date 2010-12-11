using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vocola
{
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            lblVersion.Text = "Vocola " + Vocola.VersionString;
        }

        private void lnkVocolaWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://vocola.net");
        }
    }
}
