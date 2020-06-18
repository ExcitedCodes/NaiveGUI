using System;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using NaiveGUI.Data;
using NaiveGUI.Helper;

namespace NaiveGUI
{
    /// <summary>
    /// AddRemoteWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddRemoteWindow : Window, INotifyPropertyChanged
    {
        public RemoteConfig Config = null;
        public RemoteConfigGroup Group = null;

        public Prop<string> RemoteName { get; set; } = new Prop<string>();
        public Prop<string> RemoteURI { get; set; } = new Prop<string>();
        public Prop<string> ExtraHeaders { get; set; } = new Prop<string>();

        public AddRemoteWindow(RemoteConfig config)
        {
            InitializeComponent();
            DataContext = this;
            Config = config;

            RemoteName.Value = config.Name;
            RemoteURI.Value = config.Remote.ToString();
            ExtraHeaders.Value = config.ExtraHeaders == null ? "" : string.Join(Environment.NewLine, config.ExtraHeaders);

            Title = MainWindow.GetLocalized("AddRemote_EditTitle");
            text_add.Text = MainWindow.GetLocalized("AddRemote_EditSave");
        }

        public AddRemoteWindow(RemoteConfigGroup group, string name = null, string uri = null, string extra_headers = null)
        {
            InitializeComponent();
            DataContext = this;
            Group = group;

            RemoteName.Value = name ?? "";
            RemoteURI.Value = uri ?? "";
            ExtraHeaders.Value = extra_headers ?? "";
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (RemoteName.Value.Trim() == "" || RemoteURI.Value.Trim() == "")
            {
                return;
            }
            if (Config != null)
            {
                Config.Remote = new UriBuilder(RemoteURI.Value);
                Config.ExtraHeaders = RemoteConfig.ParseExtraHeaders(ExtraHeaders.Value);
                if (RemoteName.Value != Config.Name)
                {
                    var g = Config.Group;
                    for (int i = 0; i < g.Count; i++)
                    {
                        if (g[i].Name == RemoteName.Value)
                        {
                            g.RemoveAt(i--);
                        }
                    }
                    Config.Name = RemoteName.Value;
                }
            }
            else
            {
                for (int i = 0; i < Group.Count; i++)
                {
                    if (Group[i].Name == RemoteName.Value)
                    {
                        Group.RemoveAt(i--);
                    }
                }
                Group.Add(new RemoteConfig(RemoteName.Value, ProxyType.NaiveProxy)
                {
                    Remote = new UriBuilder(RemoteURI.Value),
                    ExtraHeaders = RemoteConfig.ParseExtraHeaders(ExtraHeaders.Value)
                });
            }
            MainWindow.Instance.Save();
            Close();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
