using System;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using NaiveGUI.Data;
using NaiveGUI.Model;

namespace NaiveGUI.View
{
    /// <summary>
    /// ProxyTab.xaml 的交互逻辑
    /// </summary>
    public partial class ProxyTab : UserControl
    {
        private readonly MainViewModel Model;

        public ProxyTab(MainViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;
        }

        private void ButtonAddListener_Click(object sender, RoutedEventArgs e)
        {
            #region LOLLLLLLL

            var uri = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainViewModel.GetLocalized("YAAAY_3"), "socks://0.0.0.0:1080");
            if(uri == "")
            {
                return;
            }

            #endregion

            Model.Listeners.Insert(Model.Listeners.Count - 1, new ListenerModel(Model, uri));
            Model.Save();
        }

        private void Listener_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.Source is FrameworkElement el && (string)el.Tag == "XD")
            {
                return;
            }
            var border = sender as Border;
            if(border.DataContext is ListenerModel l)
            {
                VisualStateManager.GoToElementState(border, "Selected", true);
                Model.CurrentListener = Model.CurrentListener == l ? null : l;
                SayWTF();
            }
        }

        private void Listener_MouseEvent(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if(border.DataContext is ListenerModel l && !l.Selected)
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

        private void Listener_Initialized(object sender, EventArgs e)
        {
            var border = sender as Border;
            if(border.DataContext is ListenerModel l)
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
            if(textblock.DataContext is ListenerModel l)
            {
                l.Enabled = !l.Enabled;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if((sender as Button).DataContext is ListenerModel l)
            {
                if(l.Enabled)
                {
                    l.Enabled = false;
                    l.Stop();
                }
                Model.Listeners.Remove(l);
                Model.Save();
            }
        }

        #region WTF, TODO: remove

        bool WTFing = false;

        public void SayWTF()
        {
            WTFing = true;
            foreach(RemoteGroupModel g in Model.Remotes)
            {
                foreach(RemoteModel r in g)
                {
                    r.Selected = Model.CurrentListener != null && Model.CurrentListener.Remote == r;
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
            if(WTF.SelectedItem is RemoteModel r && Model.CurrentListener != null)
            {
                Model.CurrentListener.Remote = r;
                Model.Save();
            }
            SayWTF();
        }

        #endregion

        private void TextBlockRemote_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                new AddRemoteWindow((RemoteModel)(sender as TextBlock).DataContext).ShowDialog();
            }
        }

        private void TextBlockRemoteGroup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                new AddRemoteWindow((RemoteGroupModel)(sender as TextBlock).DataContext).ShowDialog();
            }
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            if(menu.DataContext is RemoteModel r)
            {
                r.Group.Remove(r);
                Model.Save();
            }
            else if(menu.DataContext is RemoteGroupModel g)
            {
                if(g.Count == 0 || MessageBox.Show(string.Format(MainViewModel.GetLocalized("Message_DeleteGroup"), g.Name, g.Count), "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    Model.Remotes.Remove(g);
                }
            }
        }

        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            if((sender as MenuItem).DataContext is RemoteGroupModel g)
            {
                var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainViewModel.GetLocalized("YAAAY_8"), g.Name);
                if(name == "" || name == g.Name)
                {
                    return;
                }
                foreach(var search in Model.Remotes)
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
                        Model.Remotes.Remove(g);
                        Model.Save();
                        return;
                    }
                }
                g.Name = name;
                Model.Save();
            }
        }

        private void MenuItemEdit_Click(object sender, RoutedEventArgs e) => new AddRemoteWindow((RemoteModel)(sender as MenuItem).DataContext).ShowDialog();

        private void MenuItemAddRemote_Click(object sender, RoutedEventArgs e) => new AddRemoteWindow((RemoteGroupModel)(sender as MenuItem).DataContext).ShowDialog();

        private void MenuItemAddGroup_Click(object sender, RoutedEventArgs e)
        {
            var name = App.YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(MainViewModel.GetLocalized("YAAAY_9"), "", "Add Group");
            if (name == "")
            {
                return;
            }
            foreach (var search in Model.Remotes)
            {
                if (search.Name == name)
                {
                    return;
                }
            }
            Model.Remotes.Add(new RemoteGroupModel(name, Model));
            Model.Save();
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

                new AddRemoteWindow((RemoteGroupModel)(sender as MenuItem).DataContext, name, uri.ToString(), extra_headers).ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
