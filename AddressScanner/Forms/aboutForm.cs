using AddressScanner.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace AddressScanner.Forms
{
    public partial class aboutForm : Form
    {
        public aboutForm()
        {
            InitializeComponent();
        }

        private void aboutForm_Load(object sender, EventArgs e)
        {
            Icon = Resources.Icojam_Blue_Bits_Globe_search;
            Text = "About - Address Scanner " + Application.ProductVersion;

            label3.Text = "Version : " + Application.ProductVersion;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/yasinvs");
        }
    }
}