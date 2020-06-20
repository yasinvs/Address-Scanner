using proxytester.Properties;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace proxytester
{
    public partial class Form1 : Form
    {
        internal ListViewItem _listviewadd = new ListViewItem();
        private readonly ImageList _imagelist1 = new ImageList();

        int pingSuccess;
        int pingError;
        int pingWarning;
        int count;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Text File|*.txt",
                Title = "Text File Open",
            };

            DialogResult dialogResult = openFileDialog.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {

                pingSuccess = 0;
                pingError = 0;
                pingWarning = 0;
                count = 0;

                label1.Text = "= " + Convert.ToString(pingSuccess);
                label2.Text = "= " + Convert.ToString(pingError);
                label3.Text = "= " + Convert.ToString(pingWarning);

                listView1.Items.Clear();

                SettingsClass.txtPath = openFileDialog.FileName;
                string[] array = File.ReadAllLines(SettingsClass.txtPath);

                foreach (var item in array)
                {
                    string itemString = item;
                    if (listView1.FindItemWithText(itemString) == null)
                    {
                        if(itemString.Contains(":"))
                        {
                            itemString = itemString.Substring(0, itemString.LastIndexOf(":")).Trim();
                        }
                        ListViewItem listViewItem = new ListViewItem(itemString);
                        listView1.Items.Add(listViewItem);
                    }
                }
                count = listView1.Items.Count;
                label4.Text = "= " + Convert.ToString(count);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region ImageList
            _imagelist1.Images.Add("ok", Resources.symbol_check_icon);
            _imagelist1.Images.Add("error", Resources.symbol_delete_icon);
            _imagelist1.Images.Add("warning", Resources.warning_icon);

            listView1.SmallImageList = _imagelist1;
            listView1.View = View.Details;
            #endregion
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Ping myPing = new Ping();

            pingSuccess = 0;
            pingError = 0;
            pingWarning = 0;

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if(backgroundWorker1.CancellationPending == true)
                {
                    button1.Text = "Start Now";
                    MessageBox.Show(this, "Operation is Cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
                try
                {
                    PingReply reply = myPing.Send(listView1.Items[i].Text, 2000);

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
                    }
                }
                catch (PingException)
                {
                    pingError++;
                    label2.Text = "= " + Convert.ToString(pingError);
                    listView1.Items[i].SubItems.Add("Machine Not Found");
                }
            }
            if(backgroundWorker1.CancellationPending == false)
            {
                button1.Text = "Start Now";
                MessageBox.Show(this, "Operation Completed", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void deleteSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                listView1.SelectedItems[i].Remove();
                count--;
                label4.Text = "= " + listView1.Items.Count;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy == false && listView1.Items.Count != 0)
            {
                button1.Text = "Stop";
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                backgroundWorker1.CancelAsync();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                for (int i2 = 0; i2 < listView1.Items[i].SubItems.Count; i2++)
                {
                    if(i2 != 0)
                    {
                        listView1.Items[i].SubItems[i2].Text = "";
                        listView1.Items[i].ImageIndex = -1;
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
}
