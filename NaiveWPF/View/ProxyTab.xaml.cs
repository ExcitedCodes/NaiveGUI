using System;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NaiveGUI.Data;

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

            var uri = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainWindow.GetLocalized("YAAAY_3"), "socks://0.0.0.0:1080");
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
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                new AddRemoteWindow((RemoteConfig)(sender as TextBlock).DataContext).ShowDialog();
            }
        }

        private void TextBlockRemoteGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                new AddRemoteWindow((RemoteConfigGroup)(sender as TextBlock).DataContext).ShowDialog();
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
                if(g.Count == 0 || MessageBox.Show(string.Format(MainWindow.GetLocalized("Message_DeleteGroup"), g.Name, g.Count), "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Main.Remotes.Remove(g);
                }
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuItem).DataContext is RemoteConfigGroup g)
            {
                var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainWindow.GetLocalized("YAAAY_8"), g.Name);
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

        private void MenuItemEdit_Click(object sender, RoutedEventArgs e) => new AddRemoteWindow((RemoteConfig)(sender as MenuItem).DataContext).ShowDialog();

        private void MenuItemAddRemote_Click(object sender, RoutedEventArgs e) => new AddRemoteWindow((RemoteConfigGroup)(sender as MenuItem).DataContext).ShowDialog();

        private void MenuItemAddGroup_Click(object sender, RoutedEventArgs e)
        {
            var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainWindow.GetLocalized("YAAAY_9"), "", "Add Group");
            if (name == "")
            {
                return;
            }
            foreach (var search in Main.Remotes)
            {
                if (search.Name == name)
                {
                    return;
                }
            }
            Main.Remotes.Add(new RemoteConfigGroup(name));
            Main.Save();
        }

        private void MenuItemImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new UriBuilder(Clipboard.GetText());
                var query = HttpUtility.ParseQueryString(uri.Query);

                var name = query["name"];
                var extra_headers = query["extra_headers"];

                query.Remove("name");
                query.Remove("padding");
                query.Remove("extra_headers");

                uri.Query = query.Count == 0 ? null : query.ToString();

                new AddRemoteWindow((RemoteConfigGroup)(sender as MenuItem).DataContext, name, uri.ToString(), extra_headers).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
