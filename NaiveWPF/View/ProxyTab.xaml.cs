using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NaiveGUI.Data;
using System;

namespace NaiveGUI.View
{
    /// <summary>
    /// ProxyTab.xaml 的交互逻辑
    /// </summary>
    public partial class ProxyTab : UserControl
    {
        private readonly MainWindow Main = null;

        public ProxyTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }

        private void ButtonAddListener_Click(object sender, RoutedEventArgs e)
        {
            #region LOLLLLLLL

            var uri = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input the listening URI\n\nExample:\nsocks://0.0.0.0:1080\nhttp://127.0.0.1:8388", "socks://0.0.0.0:1080");
            if(uri == "")
            {
                return;
            }

            #endregion

            Main.Listeners.Insert(Main.Listeners.Count - 1, new Listener(uri, ProxyType.NaiveProxy));
            Main.Save();
        }

        private void Listener_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.Source is FrameworkElement el && (string)el.Tag == "XD")
            {
                return;
            }
            var border = sender as Border;
            if(border.DataContext is Listener l)
            {
                VisualStateManager.GoToElementState(border, "Selected", true);
                Main.CurrentListener = Main.CurrentListener == l ? null : l;
                SayWTF();
            }
        }

        private void Listener_MouseEvent(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if(border.DataContext is Listener l && !l.Selected)
            {
                if(border.IsMouseOver)
                {
                    VisualStateManager.GoToElementState(border, "Hover", true);
                }
                else
                {
                    VisualStateManager.GoToElementState(border, "Default", true);
                }
            }
        }

        private void Listener_Initialized(object sender, System.EventArgs e)
        {
            var border = sender as Border;
            if(border.DataContext is Listener l)
            {
                l.PropertyChanged += (s, e_) =>
                {
                    if(e_.PropertyName == "Selected" && !l.Selected)
                    {
                        VisualStateManager.GoToElementState(border, "Default", true);
                    }
                };
            }
        }

        private void ListenerToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textblock = sender as TextBlock;
            if(textblock.DataContext is Listener l)
            {
                l.ToggleEnabled();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).DataContext is Listener l)
            {
                if(l.Enabled)
                {
                    l.ToggleEnabled();
                }
                Main.Listeners.Remove(l);
                Main.Save();
            }
        }

        private void AddRemote(object context)
        {
            if(context is RemoteConfigGroup g)
            {
                var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input name of remote.\nIf there's remote with same name, it will be overrided.");
                if(name == "")
                {
                    return;
                }
                var uri = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input remote uri.\n\nExample:\nhttps://user:pass@www.baidu.com:6666");
                if(uri == "")
                {
                    return;
                }
                bool padding = false;
                switch(MessageBox.Show("Enable padding?", "Another Thing", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk))
                {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    padding = true;
                    break;
                }
                for(int i = 0;i < g.Count;i++)
                {
                    if(g[i].Name == name)
                    {
                        g.RemoveAt(i--);
                    }
                }
                g.Add(new RemoteConfig(name, ProxyType.NaiveProxy)
                {
                    Remote = new UriBuilder(uri),
                    Padding = padding
                });
                Main.Save();
            }
        }

        private void EditRemote(object context)
        {
            if(context is RemoteConfig r)
            {
                var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input new name of remote.\nIf there's remote with same name, it will be overrided.", r.Name);
                if(name == "")
                {
                    return;
                }
                var uri = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input new remote uri.\n\nExample:\nhttps://user:pass@www.baidu.com:6666", r.Remote.ToString());
                if(uri == "")
                {
                    return;
                }
                bool padding = false;
                switch(MessageBox.Show("Enable padding?Current status: " + (r.Padding ? "Enabled" : "Disabled"), "Another Thing", MessageBoxButton.YesNoCancel, MessageBoxImage.Asterisk))
                {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    padding = true;
                    break;
                }
                r.Remote = new UriBuilder(uri);
                r.Padding = padding;
                if(name != r.Name)
                {
                    var g = r.Group;
                    for(int i = 0;i < g.Count;i++)
                    {
                        if(g[i].Name == name)
                        {
                            g.RemoveAt(i--);
                        }
                    }
                    r.Name = name;
                }
                Main.Save();
            }
        }

        bool WTFing = false;

        // TODO: Remove this
        public void SayWTF()
        {
            WTFing = true;
            foreach(RemoteConfigGroup g in Main.Remotes)
            {
                foreach(RemoteConfig r in g)
                {
                    r.Selected = Main.CurrentListener != null && Main.CurrentListener.Remote == r;
                }
            }
            WTFing = false;
        }

        private void WTF_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(WTFing)
            {
                return;
            }
            if(WTF.SelectedItem is RemoteConfig r && Main.CurrentListener != null)
            {
                Main.CurrentListener.Remote = r;
                Main.Save();
            }
            SayWTF();
        }

        private void TextBlockRemote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                EditRemote((sender as TextBlock).DataContext);
            }
        }

        private void TextBlockRemoteGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                AddRemote((sender as TextBlock).DataContext);
            }
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            if(menu.DataContext is RemoteConfig r)
            {
                r.Group.Remove(r);
                Main.Save();
            }
            else if(menu.DataContext is RemoteConfigGroup g)
            {
                if(g.Count == 0 || MessageBox.Show("Are you sure you want to delete group " + g.Name + " and " + g.Count + " remotes inside it?", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Main.Remotes.Remove(g);
                }
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuItem).DataContext is RemoteConfigGroup g)
            {
                var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY("Input the new name.\nIf there's group with same name, they will be merged.\nItems with same name will be overrided!", g.Name);
                if(name == "" || name == g.Name)
                {
                    return;
                }
                foreach(var search in Main.Remotes)
                {
                    if(search.Name == name)
                    {
                        foreach(var r in g)
                        {
                            foreach(var rsearch in search)
                            {
                                if(rsearch.Name == r.Name)
                                {
                                    search.Remove(rsearch);
                                    break;
                                }
                            }
                            search.Add(r);
                        }
                        Main.Remotes.Remove(g);
                        Main.Save();
                        return;
                    }
                }
                g.Name = name;
                Main.Save();
            }
        }

        private void MenuItemEdit_Click(object sender, RoutedEventArgs e) => EditRemote((sender as MenuItem).DataContext);

        private void MenuItemAddRemote_Click(object sender, RoutedEventArgs e) => AddRemote((sender as MenuItem).DataContext);
    }
}
