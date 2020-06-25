using AddressScanner.Forms;
using AddressScanner.Properties;
using AutoUpdaterDotNET;
using System;
using System.Collections;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace AddressScanner
{
    public partial class Form1 : Form
    {
        internal ListViewItem _listviewadd = new ListViewItem();
        private readonly ImageList _imagelist1 = new ImageList();
        private ColumnHeader _sortingColumn;

        #region Listview Sırala

        // Compares two ListView items based on a selected column.
        private class ListViewComparer : IComparer
        {
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            private int _columnNumber;

            // ReSharper disable once FieldCanBeMadeReadOnly.Local
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
                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (itemX.SubItems.Count <= _columnNumber)
                {
                    stringX = "";
                }
                else
                {
                    stringX = itemX.SubItems[_columnNumber].Text;
                }

                string stringY;
                // ReSharper disable once PossibleNullReferenceException
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
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
                        // ReSharper disable once StringCompareToIsCultureSpecific
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

        private int pingSuccess;
        private int pingError;
        private int pingWarning;
        private int count;

        internal void EnableDisable(bool value)
        {
            switch (value)
            {
                case true:
                    fileToolStripMenuItem.Enabled = true;
                    listToolStripMenuItem.Enabled = true;
                    aboutToolStripMenuItem.Enabled = true;
                    button1.Enabled = true;
                    break;

                case false:
                    fileToolStripMenuItem.Enabled = false;
                    listToolStripMenuItem.Enabled = false;
                    aboutToolStripMenuItem.Enabled = false;
                    button1.Enabled = false;
                    break;
            }
        }

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileToolStripMenuItem.Enabled == true)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog()
                {
                    Filter = "Text File|*.txt",
                    Title = "Text File Open",
                };

                DialogResult dialogResult = openFileDialog.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    SettingsClass.txtPath = openFileDialog.FileName;
                    if (bckTest.IsBusy != true && bckOpen.IsBusy != true)
                    {
                        bckOpen.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show(this, "Wait for the current transaction to finish !", "Please Wait !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Icon = Resources.Icojam_Blue_Bits_Globe_search;

            #region ImageList

            _imagelist1.Images.Add("ok", Resources.symbol_check_icon);
            _imagelist1.Images.Add("error", Resources.symbol_delete_icon);
            _imagelist1.Images.Add("warning", Resources.warning_icon);

            listView1.SmallImageList = _imagelist1;
            listView1.View = View.Details;

            #endregion ImageList

            AutoUpdater.Start("https://raw.githubusercontent.com/yasinvs/Address-Scanner/master/AddressScanner/_/version.xml");
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Ping myPing = new Ping();

            pingSuccess = 0;
            pingError = 0;
            pingWarning = 0;

            EnableDisable(false);
            button1.Enabled = true;

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (bckTest.CancellationPending == true)
                {
                    button1.Text = "Start Now";
                    toolStripStatusLabel1.Text = "Operation is Cancelled";
                    MessageBox.Show(this, "Operation is Cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
                try
                {
                    PingReply reply = myPing.Send(listView1.Items[i].Text, 2000);

                    toolStripStatusLabel1.Text = "Testing : " + listView1.Items[i].Text;

                    if (reply != null)
                    {
                        try
                        {
                            listView1.Items[i].SubItems[1].Text = Convert.ToString(reply.Address);
                            listView1.Items[i].SubItems[2].Text = Convert.ToString(reply.Status);
                            listView1.Items[i].SubItems[3].Text = Convert.ToString(reply.RoundtripTime);
                        }
                        catch { }

                        listView1.Items[i].SubItems.Add(Convert.ToString(reply.Address));
                        listView1.Items[i].SubItems.Add(Convert.ToString(reply.Status));
                        listView1.Items[i].SubItems.Add(Convert.ToString(reply.RoundtripTime));

                        switch (reply.Status)
                        {
                            case IPStatus.Success:
                                pingSuccess++;
                                listView1.Items[i].ImageIndex = 0;
                                break;

                            case IPStatus.DestinationNetworkUnreachable:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.DestinationHostUnreachable:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.DestinationProtocolUnreachable:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.DestinationPortUnreachable:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.NoResources:
                                break;

                            case IPStatus.BadOption:
                                break;

                            case IPStatus.HardwareError:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.PacketTooBig:
                                break;

                            case IPStatus.TimedOut:
                                pingWarning++;
                                listView1.Items[i].ImageIndex = 2;
                                break;

                            case IPStatus.BadRoute:
                                break;

                            case IPStatus.TtlExpired:
                                break;

                            case IPStatus.TtlReassemblyTimeExceeded:
                                break;

                            case IPStatus.ParameterProblem:
                                break;

                            case IPStatus.SourceQuench:
                                break;

                            case IPStatus.BadDestination:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.DestinationUnreachable:
                                pingError++;
                                listView1.Items[i].ImageIndex = 1;
                                break;

                            case IPStatus.TimeExceeded:
                                break;

                            case IPStatus.BadHeader:
                                break;

                            case IPStatus.UnrecognizedNextHeader:
                                break;

                            case IPStatus.IcmpError:
                                break;

                            case IPStatus.DestinationScopeMismatch:
                                break;

                            case IPStatus.Unknown:
                                break;
                        }

                        label1.Text = "= " + Convert.ToString(pingSuccess);
                        label2.Text = "= " + Convert.ToString(pingError);
                        label3.Text = "= " + Convert.ToString(pingWarning);

                        if (checkBox1.CheckState == CheckState.Checked)
                        {
                            listView1.EnsureVisible(i);
                        }
                    }
                }
                catch (PingException)
                {
                    pingError++;
                    label2.Text = "= " + Convert.ToString(pingError);
                    listView1.Items[i].SubItems.Add("Machine Not Found");
                }
            }
            if (bckTest.CancellationPending == false)
            {
                button1.Text = "Start Now";
                toolStripStatusLabel1.Text = "Operation is Completed";
                MessageBox.Show(this, "Operation is Completed", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            EnableDisable(true);
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
                        label4.Text = "= " + listView1.Items.Count;
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
                    button1.Text = "Stop";
                    bckTest.RunWorkerAsync();
                }
                else
                {
                    SettingsClass.count = 0;
                    DialogResult dialogResult = MessageBox.Show(this, "Delete previous information?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult == DialogResult.Yes)
                    {
                        clearToolStripMenuItem_Click(sender, e);
                        goto start;
                    }
                    else
                    {
                        goto start;
                    }
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
                DialogResult dialogResult = MessageBox.Show(this, "Are you sure?", "Clear (Name Exclude)", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        for (int i2 = 0; i2 < listView1.Items[i].SubItems.Count; i2++)
                        {
                            if (i2 != 0)
                            {
                                listView1.Items[i].SubItems[i2].Text = "";
                                listView1.Items[i].ImageIndex = -1;
                            }
                        }
                    }
                }

                pingSuccess = 0;
                pingError = 0;
                pingWarning = 0;

                label1.Text = "= " + Convert.ToString(pingSuccess);
                label2.Text = "= " + Convert.ToString(pingError);
                label3.Text = "= " + Convert.ToString(pingWarning);
            }
        }

        private void bckOpen_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            EnableDisable(false);

            pingSuccess = 0;
            pingError = 0;
            pingWarning = 0;
            count = 0;

            label1.Text = "= " + Convert.ToString(pingSuccess);
            label2.Text = "= " + Convert.ToString(pingError);
            label3.Text = "= " + Convert.ToString(pingWarning);

            listView1.Items.Clear();

            string[] array = File.ReadAllLines(SettingsClass.txtPath);

            foreach (var item in array)
            {
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

                if (checkBox1.CheckState == CheckState.Checked)
                {
                    listView1.EnsureVisible(listView1.Items.Count - 1);
                }
            }
            count = listView1.Items.Count;
            label4.Text = "= " + Convert.ToString(count);

            EnableDisable(true);
            toolStripStatusLabel1.Text = "List Opened !";
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
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
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm about = new aboutForm();

            about.ShowDialog();
        }
    }
}