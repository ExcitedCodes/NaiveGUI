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

        bool WTFing = false;

        // TODO: Remove this
        public void SayWTF()
        {
            WTFing = true;
            foreach(RemoteConfigGroup g in Main.Remotes)
            {
                foreach(RemoteConfig r in g)
                {
                    r.Selected.Value = Main.CurrentListener != null && Main.CurrentListener.Remote == r;
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
    }
}
