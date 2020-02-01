using System;
using System.Windows.Forms;

namespace NaiveGUI.Subscription
{
    public partial class SubscriptionForm : Form
    {
        MainForm Main = MainForm.Instance;

        public SubscriptionForm()
        {
            InitializeComponent();
            Icon = MainForm.Instance.Icon;

            checkBox_auto_update.Checked = textBox_interval.Enabled = Main.Subscriptions.UpdateInterval >= 60;
            textBox_interval.Text = Main.Subscriptions.UpdateInterval.ToString();

            RefreshItems();
        }

        private SubscriptionData AddItem(SubscriptionData s) => (SubscriptionData)(listView_sub.Items.Add(new ListViewItem(new string[]
        {
            s.URL,
            s.GetLastUpdate()
        })).Tag = s);

        private bool ValidateInput(ListViewItem whitelist = null)
        {
            if((textBox_url.Text = textBox_url.Text.Trim()) != "")
            {
                foreach(ListViewItem i in listView_sub.Items)
                {
                    if(i.SubItems[0].Text == textBox_url.Text && i != whitelist)
                    {
                        MessageBox.Show("URL already exists!", "Oof", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void RefreshItems()
        {
            listView_sub.BeginUpdate();
            listView_sub.Items.Clear();
            foreach(var s in Main.Subscriptions)
            {
                AddItem(s);
            }
            listView_sub.EndUpdate();
        }

        #region General Events

        private void SubscriptionForm_Load(object sender, EventArgs e) { }

        private void listView_sub_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_save.Visible = listView_sub.SelectedItems.Count == 1;
            if(button_save.Visible)
            {
                textBox_url.Text = listView_sub.SelectedItems[0].SubItems[0].Text;
            }
        }

        private void listView_sub_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete && listView_sub.SelectedItems.Count > 0 && MessageBox.Show("Are you sure you want to delete " + listView_sub.SelectedItems.Count + "item(s)?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                foreach(ListViewItem i in listView_sub.SelectedItems)
                {
                    listView_sub.Items.Remove(i);
                    Main.Subscriptions.Remove((SubscriptionData)i.Tag);
                }
                Main.Save();
            }
        }

        private void textBox_url_KeyDown(object sender, KeyEventArgs e)
        {
            // Use KeyDown here so if user use Enter to close the error dialog, it won't trigger add
            if(e.KeyCode == Keys.Enter)
            {
                if(button_save.Visible)
                {
                    button_save.PerformClick();
                }
                else
                {
                    button_add.PerformClick();
                }
            }
        }

        private void textBox_interval_TextChanged(object sender, EventArgs e) => button_save_global.Enabled = true;

        private void checkBox_auto_update_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox_auto_update.Checked)
            {
                textBox_interval.Enabled = true;
                textBox_interval.Text = "7200";
            }
            else
            {
                textBox_interval.Enabled = false;
                textBox_interval.Text = "-1";
            }
            button_save_global.Enabled = true;
        }

        #endregion

        #region Button Events

        private void button_add_Click(object sender, EventArgs e)
        {
            if(ValidateInput())
            {
                Main.Subscriptions.Add(AddItem(new SubscriptionData()
                {
                    URL = textBox_url.Text
                }));
                textBox_url.Clear();
                Main.Save();
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if(ValidateInput(listView_sub.SelectedItems[0]))
            {
                var sub = (SubscriptionData)listView_sub.SelectedItems[0].Tag;
                sub.LastUpdate = DateTime.MinValue;
                sub.URL = listView_sub.SelectedItems[0].SubItems[0].Text = textBox_url.Text;
                listView_sub.SelectedItems.Clear();
                textBox_url.Clear();
                Main.Save();
            }
        }

        private void button_save_global_Click(object sender, EventArgs e)
        {
            if((!int.TryParse(textBox_interval.Text.Trim(), out int interval) || interval < 60) && interval != -1)
            {
                MessageBox.Show("Please enter a valid and >=60 number!", "Opps", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Main.Subscriptions.UpdateInterval = interval;
            Main.Save();
            button_save_global.Enabled = false;
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            // TODO: Should be async
            Main.Subscriptions.Update(false, true);
            RefreshItems();
        }

        #endregion
    }
}
