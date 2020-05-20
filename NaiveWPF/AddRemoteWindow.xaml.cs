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
        public Prop<bool> EnablePadding { get; set; } = new Prop<bool>();

        public AddRemoteWindow(RemoteConfig config)
        {
            InitializeComponent();
            DataContext = this;
            Config = config;

            RemoteName.Value = config.Name;
            RemoteURI.Value = config.Remote.ToString();
            EnablePadding.Value = config.Padding;

            Title = MainWindow.GetLocalized("AddRemote_EditTitle");
            text_add.Text = MainWindow.GetLocalized("AddRemote_EditSave");
        }

        public AddRemoteWindow(RemoteConfigGroup group, string name = null, string uri = null, bool padding = false)
        {
            InitializeComponent();
            DataContext = this;
            Group = group;

            RemoteName.Value = name ?? "";
            RemoteURI.Value = uri ?? "";
            EnablePadding.Value = padding;
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
                Config.Padding = EnablePadding.Value;
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
                    Padding = EnablePadding.Value
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
