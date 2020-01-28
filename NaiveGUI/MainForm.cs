using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NaiveGUI
{
    /// <summary>
    /// MainForm在这里扮演整个应用程序实例的角色
    /// 应该是一个单实例的对象并且可以在任何地方用 <see cref="Instance"/> 获取
    /// </summary>
    public partial class MainForm : Form
    {
        public static MainForm Instance = null;

        public RemoteConfig CurrentRemote = null;
        public ProxyListener CurrentListener = null;

        public List<RemoteConfig> Remotes = new List<RemoteConfig>();
        public List<ProxyListener> Listeners = new List<ProxyListener>();

        public MainForm()
        {
            if(Instance != null)
            {
                throw new Exception("WTF dude");
            }
            Instance = this;
            InitializeComponent();
        }

        public void Save()
        {

        }

        #region UI Refresh Functions

        /// <summary>
        /// 加载一个 <see cref="RemoteConfig"/> 并将其中的设置展示到当前配置UI上
        /// </summary>
        public void LoadConfigUI(RemoteConfig config)
        {
            CurrentRemote = config;
            groupBox1.Enabled = config != null;
            if(!groupBox1.Enabled)
            {
                return;
            }
            textBox_name.Text = config.Name;
            textBox_group.Text = config.Group;
            comboBox_protocol.Text = config.Remote.Scheme;
            textBox_host.Text = config.Remote.Host + (config.Remote.Port != -1 ? (":" + config.Remote.Port) : "");
            textBox_username.Text = config.Remote.UserName;
            textBox_password.Text = config.Remote.Password;
            checkBox_padding.Checked = config.Padding;
            comboBox_quic.Text = config.QuicVersion == -1 ? "-" : config.QuicVersion.ToString();
        }

        /// <summary>
        /// 更新 <see cref="listView_listeners"/> 使其显示与当前的 <see cref="Listeners"/> 匹配
        /// </summary>
        public void RefreshListenerList()
        {
            listView_listeners.BeginUpdate();
            listView_listeners.Items.Clear();
            foreach(var l in Listeners)
            {
                var item = listView_listeners.Items.Add(l.Listen.ToString());
                item.Checked = l.Working;
                item.Tag = l;
            }
            listView_listeners.EndUpdate();
            if(listView_listeners.Items.Count > 0)
            {
                listView_listeners.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// 更新 <see cref="tree_remotes"/> 使其显示与当前的 <see cref="Remotes"/> 匹配
        /// </summary>
        public void RefreshRemoteTree(bool expandAll)
        {
            var state = expandAll ? null : tree_remotes.Nodes.GetExpansionState();
            tree_remotes.BeginUpdate();
            tree_remotes.Nodes.Clear();
            foreach(var r in Remotes)
            {
                if(!tree_remotes.Nodes.ContainsKey(r.Group))
                {
                    var p = tree_remotes.Nodes.Add(r.Group, r.Group);
                    p.ImageKey = p.SelectedImageKey = "Dots";
#if HORSE
                    if(r.Group.EndsWith("horse"))
                    {
                        p.ImageKey = p.SelectedImageKey = "Horse";
                    }
#endif
                }
                var group = tree_remotes.Nodes[r.Group];
                if(!group.Nodes.ContainsKey(r.Name))
                {
                    var node = group.Nodes.Add(r.Name, r.Name);
                    node.Tag = r;
                    node.SelectedImageKey = node.ImageKey = "Naive";
                }
            }
            if(expandAll)
            {
                tree_remotes.ExpandAll();
            }
            else
            {
                tree_remotes.Nodes.SetExpansionState(state);
            }
            RefreshRemoteTreeCheckStatus();
            tree_remotes.EndUpdate();
        }

        /// <summary>
        /// 更新 <see cref="tree_remotes"/> 中项目的勾选状态, 使其显示与 <see cref="CurrentListener.Remote"/> 匹配
        /// </summary>
        public void RefreshRemoteTreeCheckStatus()
        {
            if(CurrentListener != null)
            {
                foreach(var i in tree_remotes.Nodes.Descendants())
                {
                    // TODO: Check parent
                    if(i.Tag is RemoteConfig cfg)
                    {
                        i.Checked = CurrentListener.Remote == cfg;
                    }
                }
            }
        }

        #endregion

        #region General Events

        private void MainForm_Load(object sender, EventArgs e) { }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // TODO: Tray
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach(var l in Listeners)
            {
                l.Stop();
            }
        }

        private void tree_remotes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if(CurrentListener == null)
            {
                return;
            }
            if(e.Node.Parent == null)
            {
                if(e.Node.Checked)
                {
                    e.Node.Checked = false;
                }
            }
            else if(e.Node.Tag is RemoteConfig cfg)
            {
                if(!e.Node.Checked && CurrentListener.Remote == cfg)
                {
                    e.Node.Checked = true;
                }
                else if(e.Node.Checked)
                {
                    CurrentListener.Remote = cfg;
                }
                // TODO: Save
            }
            if(e.Action != TreeViewAction.Unknown)
            {
                RefreshRemoteTreeCheckStatus();
            }
        }

        private void tree_remotes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Tag is RemoteConfig cfg)
            {
                LoadConfigUI(cfg);
            }
        }

        private void listView_listeners_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
           
        }

        private void listView_listeners_SelectedIndexChanged(object sender, EventArgs e)
        {
            tree_remotes.CheckBoxes = listView_listeners.SelectedItems.Count != 0;
            if(tree_remotes.CheckBoxes && listView_listeners.SelectedItems[0].Tag is ProxyListener l)
            {
                CurrentListener = l;
                RefreshRemoteTreeCheckStatus();
            }
        }

        private void listView_listeners_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if(e.Item.Tag is ProxyListener l)
            {
                if(e.Item.Checked)
                {
                    l.Start();
                }
                else
                {
                    l.Stop();
                }
            }
        }

        #endregion

        #region Button Events

        private void button_save_Click(object sender, EventArgs e)
        {
            if(CurrentRemote != null)
            {
                var host = textBox_host.Text.Split(':');
                if(host.Length > 2)
                {
                    Trace.Fail("Illegal host");
                    return;
                }
                if(host.Length < 2)
                {
                    CurrentRemote.Remote.Port = -1;

                }
                else if(int.TryParse(host[1], out int port))
                {
                    CurrentRemote.Remote.Port = port;
                }
                CurrentRemote.Remote.Scheme = comboBox_protocol.Text;
                CurrentRemote.Remote.Host = host[0];
                CurrentRemote.Remote.UserName = textBox_username.Text;
                CurrentRemote.Remote.Password = textBox_password.Text;
                CurrentRemote.Padding = checkBox_padding.Checked;
                if(comboBox_quic.Text == "-")
                {
                    CurrentRemote.QuicVersion = -1;
                }
                else if(int.TryParse(comboBox_quic.Text, out int quic))
                {
                    CurrentRemote.QuicVersion = quic;
                }
                CurrentRemote.Name = textBox_name.Text;
                CurrentRemote.Group = textBox_group.Text;
            }
        }

        private void button_remote_add_Click(object sender, EventArgs e)
        {
            int i = 0;
            string group = "Default";
            if(tree_remotes.SelectedNode != null)
            {
                var node = tree_remotes.SelectedNode;
                while(node.Parent != null)
                {
                    node = node.Parent;
                }
                group = node.Text;
            }
            if(tree_remotes.Nodes.ContainsKey(group))
            {
                while(tree_remotes.Nodes[group].Nodes.ContainsKey("NewRemote" + ++i)) ;
            }
            else
            {
                i = 1;
            }
            var remote = new RemoteConfig(group, "NewRemote" + i);
            Remotes.Add(remote);
            RefreshRemoteTree(false);
            tree_remotes.Nodes[remote.Group].Expand();
            tree_remotes.SelectedNode = tree_remotes.Nodes[remote.Group].Nodes[remote.Name];
            // TODO: Save
        }

        private void button_discard_Click(object sender, EventArgs e) => LoadConfigUI(CurrentRemote);

        private void button_listener_add_Click(object sender, EventArgs e)
        {
            Listeners.Add(new ProxyListener(new UriBuilder("socks", textBox_listener_address.Text, int.TryParse(textBox_listener_port.Text, out int port) ? port : 1080)));
            // TODO: Save
            RefreshListenerList();
        }

        #endregion
    }
}
