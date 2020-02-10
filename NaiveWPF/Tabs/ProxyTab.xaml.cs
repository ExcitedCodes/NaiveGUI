using System.Windows.Controls;

namespace NaiveGUI
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
    }
}
