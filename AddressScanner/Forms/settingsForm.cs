using AddressScanner.Properties;
using System;
using System.Windows.Forms;

namespace AddressScanner.Forms
{
    public partial class settingsForm : Form
    {
        public settingsForm()
        {
            InitializeComponent();
        }

        private void formSettings_Load(object sender, EventArgs e)
        {
            Text = "Settings - Address Scanner " + Application.ProductVersion;
            Icon = Resources.Icojam_Blue_Bits_Globe_search;

            #region Load Settings

            // Check for Update(s) on Startup
            cbCheckforUpdates.Checked = Convert.ToBoolean(SettingsClass.iniFile.Sections[1].Keys[0].Value);

            // Enable AutoScroll on Startup
            cbAutoScroll.Checked = Convert.ToBoolean(SettingsClass.iniFile.Sections[0].Keys[1].Value);

            // Ping Timeout (default : 2000)
            txtTimeout.Text = SettingsClass.iniFile.Sections[0].Keys[0].Value;

            #endregion Load Settings
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Check for Update(s) on Startup
            SettingsClass.iniFile.Sections[1].Keys[0].Value = cbCheckforUpdates.Checked.ToString();

            // Enable AutoScroll on Startup
            SettingsClass.iniFile.Sections[0].Keys[1].Value = cbAutoScroll.Checked.ToString();

            // Ping Timeout (default : 2000)
            if (txtTimeout.Text != "")
            {
                SettingsClass.iniFile.Sections[0].Keys[0].Value = txtTimeout.Text;
            }
            else
            {
                SettingsClass.iniFile.Sections[0].Keys[0].Value = "2000";
            }

            SettingsClass.iniFile.Save(Application.StartupPath + "\\settings.ini");

            Hide();
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(this, "Are you sure?", "Default Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                // Check for Update(s) on Startup
                cbCheckforUpdates.Checked = true;

                // Enable AutoScroll on Startup
                cbAutoScroll.Checked = false;

                // Ping Timeout (default : 2000)
                txtTimeout.Text = "2000";
            }
        }

        private void txtTimeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}