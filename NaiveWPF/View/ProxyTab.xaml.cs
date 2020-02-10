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

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if(border.DataContext is Listener l)
            {
                Main.CurrentListener = l;
            }
        }
    }
}
