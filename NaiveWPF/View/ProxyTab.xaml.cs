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
        MainWindow Main;

        public ProxyTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }

        private void ButtonAddListener_Click(object sender, RoutedEventArgs e)
        {
            Main.Listeners.Insert(Main.Listeners.Count - 1, new Listener("socks://0.0.0.0:1080"));
            Main.Save();
        }

        private void Listener_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is TextBlock t && (string)t.Tag == "XD")
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
            if(!(border.DataContext as Listener).Selected)
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
            }
            SayWTF();
        }
    }
}
