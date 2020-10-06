using AddressScanner.Properties;
using AutoUpdaterDotNET;
using MadMilkman.Ini;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace AddressScanner.Forms
{
    public partial class mainForm : Form
    {
        #region Class-Property
        private Ping myPing = new Ping();
        internal ListViewItem _listviewadd = new ListViewItem();
        private readonly ImageList _imagelist1 = new ImageList();
        private ColumnHeader _sortingColumn;

        private int pingSuccess;
        private int pingError;
        private int pingWarning;
        private int count;
        #endregion

        #region Listview Items Sorting
        // Compares two ListView items based on a selected column.
        private class ListViewComparer : IComparer
        {
            private int _columnNumber;

            private SortOrder _sortOrder;

            public ListViewComparer(int columnNumber,
                SortOrder sortOrder)
            {
                _columnNumber = columnNumber;
                _sortOrder = sortOrder;
            }

            // Compare two ListViewItems.
            public int Compare(object objectX, object objectY)
            {
                // Get the objects as ListViewItems.
                ListViewItem itemX = objectX as ListViewItem;
                ListViewItem itemY = objectY as ListViewItem;

                // Get the corresponding sub-item values.
                string stringX;
                if (itemX.SubItems.Count <= _columnNumber)
                {
                    stringX = "";
                }
                else
                {
                    stringX = itemX.SubItems[_columnNumber].Text;
                }

                string stringY;
                if (itemY.SubItems.Count <= _columnNumber)
                {
                    stringY = "";
                }
                else
                {
                    stringY = itemY.SubItems[_columnNumber].Text;
                }

                // Compare them.
                int result;
                double doubleX, doubleY;
                if (double.TryParse(stringX, out doubleX) &&
                    double.TryParse(stringY, out doubleY))
                {
                    // Treat as a number.
                    result = doubleX.CompareTo(doubleY);
                }
                else
                {
                    DateTime dateX, dateY;
                    if (DateTime.TryParse(stringX, out dateX) &&
                        DateTime.TryParse(stringY, out dateY))
                    {
                        // Treat as a date.
                        result = dateX.CompareTo(dateY);
                    }
                    else
                    {
                        // Treat as a string.
                        result = stringX.CompareTo(stringY);
                    }
                }

                // Return the correct result depending on whether
                // we're sorting ascending or descending.
                if (_sortOrder == SortOrder.Ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }

        #endregion Listview Sırala

        #region EnableDisableItems
        internal void EnableDisableItems(bool value = false)
        {
            fileToolStripMenuItem.Enabled = value;
            listToolStripMenuItem.Enabled = value;
            applicationToolStripMenuItem.Enabled = value;
            btnStartNow.Enabled = value;
        }
        #endregion

        public mainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region List Open Button
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Text File|*.txt",
                Title = "Open List - Address Scanner " + Application.ProductVersion,
            };

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                SettingsClass.txtPath = openFileDialog.FileName;
                bckOpen.RunWorkerAsync();
            }
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region Form Properties
            Text = "Address Scanner " + Application.ProductVersion;
            Icon = Resources.Icojam_Blue_Bits_Globe_search;
            #endregion

            #region ImageList

            _imagelist1.Images.Add("ok", Resources.symbol_check_icon);
            _imagelist1.Images.Add("error", Resources.symbol_delete_icon);
            _imagelist1.Images.Add("warning", Resources.warning_icon);

            listView1.SmallImageList = _imagelist1;
            listView1.View = View.Details;

            #endregion ImageList

            #region Settings

            if (!File.Exists(Application.StartupPath + "\\settings.ini"))
            {
                IniSection sectionSettings = SettingsClass.iniFile.Sections.Add("Settings");
                IniSection sectionApplication = SettingsClass.iniFile.Sections.Add("Application");
                IniKey key;

                key = sectionSettings.Keys.Add("TimeOut", "2000");
                key = sectionSettings.Keys.Add("DefaultAutoScroll", "false");
                key = sectionApplication.Keys.Add("AutoUpdate", "true");

                SettingsClass.iniFile.Save(Application.StartupPath + "\\settings.ini");
            }

            try
            {
                SettingsClass.iniFile.Load(Application.StartupPath + "\\settings.ini");
            }
            catch(FormatException)
            {
                MessageBox.Show(this, "Since the settings file is damaged, the application will be restarted and reset.","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                if(File.Exists(Application.StartupPath + "\\settings.ini"))
                {
                    File.Delete(Application.StartupPath + "\\settings.ini");
                }
                Application.Restart();
            }


            SettingsClass.timeout = Convert.ToInt32(SettingsClass.iniFile.Sections[0].Keys[0].Value);
            cbAutoScroll.Checked = Convert.ToBoolean(SettingsClass.iniFile.Sections[0].Keys[1].Value);
            SettingsClass.autoupdate = Convert.ToBoolean(SettingsClass.iniFile.Sections[1].Keys[0].Value);

            #endregion Settings

            #region Update
            if (SettingsClass.autoupdate == true)
            {
                AutoUpdater.Start("https://raw.githubusercontent.com/yasinvs/Address-Scanner/master/AddressScanner/_/version.xml");
                AutoUpdater.RunUpdateAsAdmin = false;
            }
            #endregion

        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            #region int Count Reset
            pingSuccess = 0;
            pingError = 0;
            pingWarning = 0;
            #endregion

            #region Enable-Disable Form Items
            EnableDisableItems(false);
            btnStartNow.Enabled = true;
            #endregion

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                #region bck cancellation procedures
                if (bckTest.CancellationPending == true)
                {

                    btnStartNow.Text = "Start Now";
                    toolStripStatusLabel1.Text = "Operation is Cancelled";
                    MessageBox.Show(this, "Operation is Cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
                #endregion

                try
                {
                    PingReply reply = myPing.Send(listView1.Items[i].Text, SettingsClass.timeout);
                    toolStripStatusLabel1.Text = "Testing : " + listView1.Items[i].Text;

                    if (reply != null)
                    {
                        #region Write Information
                        try
                        {
                            listView1.Items[i].SubItems[1].Text = Convert.ToString(reply.Address);
                            listView1.Items[i].SubItems[2].Text = Convert.ToString(reply.Status);
                            listView1.Items[i].SubItems[3].Text = Convert.ToString(reply.RoundtripTime);
                        }
                        catch
                        {
                            listView1.Items[i].SubItems.Add(Convert.ToString(reply.Address));
                            listView1.Items[i].SubItems.Add(Convert.ToString(reply.Status));
                            listView1.Items[i].SubItems.Add(Convert.ToString(reply.RoundtripTime));
                        }
                        #endregion

                        #region reply Status
                        switch (reply.Status)
                        {
                            case IPStatus.Success:
                                pingSuccess++;
                                listView1.Items[i].ImageIndex = 0;
                                break;
                            case IPStatus.TimedOut:
                                pingWarning++;
                                listView1.Items[i].ImageIndex = 2;
                                break;
                            default:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;
                        }
                        #endregion

                        #region Write Ping Informations Count
                        lblSuccess.Text = "= " + Convert.ToString(pingSuccess);
                        lblError.Text = "= " + Convert.ToString(pingError);
                        lblWarning.Text = "= " + Convert.ToString(pingWarning);
                        #endregion

                        #region AutoScroll
                        if (cbAutoScroll.CheckState == CheckState.Checked)
                        {
                            listView1.EnsureVisible(i);
                        }
                        #endregion
                    }
                }
                catch (PingException)
                {
                    #region Machine Not Found Information or Other(s) Error
                    pingError++;
                    listView1.Items[i].ImageIndex = 1;
                    listView1.Items[i].SubItems.Add("Machine Not Found");
                    lblError.Text = "= " + Convert.ToString(pingError);
                    #endregion
                }
            }

            #region if Cancel control producedures
            if (bckTest.CancellationPending == false)
            {
                btnStartNow.Text = "Start Now";
                toolStripStatusLabel1.Text = "Operation is Completed";
                MessageBox.Show(this, "Operation is Completed", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion

            EnableDisableItems(true);
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listToolStripMenuItem.Enabled == true)
            {
                DialogResult dialogResult = MessageBox.Show(this, "Are you sure?", "Delete Selected", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    for (int i = 0; i < listView1.SelectedItems.Count; i++)
                    {
                        listView1.SelectedItems[i].Remove();
                        count--;
                        lblDatabaseCount.Text = "= " + listView1.Items.Count;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        start:
            if (bckTest.IsBusy == false && listView1.Items.Count != 0)
            {
                if (SettingsClass.count == 0)
                {
                    SettingsClass.count++;
                    btnStartNow.Text = "Stop";
                    bckTest.RunWorkerAsync();
                }
                else
                {
                    SettingsClass.count = 0;
                    clearToolStripMenuItem_Click(sender, e);
                    goto start;
                }
            }
            else
            {
                bckTest.CancelAsync();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listToolStripMenuItem.Enabled == true)
            {
                SettingsClass.count = 0;
                DialogResult dialogResult = MessageBox.Show(this, "Delete previous information. Are you sure?", "Clear (Name Exclude)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        for (int i2 = 0; i2 < listView1.Items[i].SubItems.Count; i2++)
                        {
                            if (i2 != 0)
                            {
                                listView1.Items[i].ImageIndex = -1;
                                listView1.Items[i].SubItems[i2].Text = "";
                            }
                        }
                    }
                }

                pingSuccess = 0;
                pingError = 0;
                pingWarning = 0;

                lblSuccess.Text = "= " + Convert.ToString(pingSuccess);
                lblError.Text = "= " + Convert.ToString(pingError);
                lblWarning.Text = "= " + Convert.ToString(pingWarning);
                listView1.Update();
            }
        }

        private void bckOpen_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EnableDisableItems(false);
            pingSuccess = 0;
            pingError = 0;
            pingWarning = 0;
            count = 0;

            lblSuccess.Text = "= " + Convert.ToString(pingSuccess);
            lblError.Text = "= " + Convert.ToString(pingError);
            lblWarning.Text = "= " + Convert.ToString(pingWarning);

            listView1.Items.Clear();

            string[] array = File.ReadAllLines(SettingsClass.txtPath);

            foreach (var item in array)
            {
                if (bckTest.CancellationPending == true)
                {
                    break;
                }
                string itemString = item;
                if (listView1.FindItemWithText(itemString) == null)
                {
                    if (itemString.Contains(":"))
                    {
                        itemString = itemString.Substring(0, itemString.LastIndexOf(":")).Trim();
                    }
                    toolStripStatusLabel1.Text = "Adding : " + itemString;
                    ListViewItem listViewItem = new ListViewItem(itemString);
                    listView1.Items.Add(listViewItem);
                }

                count = listView1.Items.Count;
                lblDatabaseCount.Text = "= " + Convert.ToString(count);

                if (cbAutoScroll.CheckState == CheckState.Checked)
                {
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                }
            }
            EnableDisableItems(true);
            toolStripStatusLabel1.Text = "List Opened !";
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            #region Column Sort
            if (bckTest.IsBusy != true && bckOpen.IsBusy != true)
            {
                // Get the new sorting column.
                ColumnHeader newSortingColumn = listView1.Columns[e.Column];

                // Figure out the new sorting order.
                SortOrder sortOrder;
                if (_sortingColumn == null)
                {
                    // New column. Sort ascending.
                    sortOrder = SortOrder.Ascending;
                }
                else
                {
                    // See if this is the same column.
                    if (newSortingColumn == _sortingColumn)
                    {
                        // Same column. Switch the sort order.
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        if (_sortingColumn.Text.StartsWith("> "))
                        {
                            sortOrder = SortOrder.Descending;
                        }
                        else
                        {
                            sortOrder = SortOrder.Ascending;
                        }
                    }
                    else
                    {
                        // New column. Sort ascending.
                        sortOrder = SortOrder.Ascending;
                    }

                    // Remove the old sort indicator.
                    _sortingColumn.Text = _sortingColumn.Text.Substring(2);
                }

                // Display the new sort order.
                _sortingColumn = newSortingColumn;
                if (sortOrder == SortOrder.Ascending)
                {
                    _sortingColumn.Text = @"> " + _sortingColumn.Text;
                }
                else
                {
                    _sortingColumn.Text = @"< " + _sortingColumn.Text;
                }

                // Create a comparer.
                listView1.ListViewItemSorter =
                    new ListViewComparer(e.Column, sortOrder);

                // Sort.
                listView1.Sort();
            }
            else
            {
                MessageBox.Show(this, "Wait for the current transaction to finish !", "Please Wait !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bckOpen.CancelAsync();
            bckTest.CancelAsync();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settingsForm settings = new settingsForm();

            settings.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            aboutForm about = new aboutForm();

            about.ShowDialog();
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //declare new SaveFileDialog + set it's initial properties
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Choose file to save to",
                Filter = "CSV (*.csv)|*.csv",
            };

            //show the dialog + display the results in a msgbox unless cancelled

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string[] headers = listView1.Columns
                           .OfType<ColumnHeader>()
                           .Select(header => header.Text.Trim())
                           .ToArray();

                string[][] items = listView1.Items
                            .OfType<ListViewItem>()
                            .Select(lvi => lvi.SubItems
                                .OfType<ListViewItem.ListViewSubItem>()
                                .Select(si => si.Text).ToArray()).ToArray();

                string table = string.Join(",", headers) + Environment.NewLine;
                foreach (string[] a in items)
                {
                    //a = a_loopVariable;
                    table += string.Join(",", a) + Environment.NewLine;
                }
                table = table.TrimEnd('\r', '\n');
                File.WriteAllText(sfd.FileName, table);
                MessageBox.Show(this, "Export completed !", "Export to CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (listView1.FindItemWithText(toolStripTextBox1.Text) == null)
                {
                    listView1.Items.Add(toolStripTextBox1.Text);
                }
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = "";
        }
    }
}