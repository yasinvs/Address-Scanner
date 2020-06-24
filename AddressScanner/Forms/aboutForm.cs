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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/yasinvs");
        }
    }
}
