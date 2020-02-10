using System.Windows.Controls;

namespace NaiveGUI
{
    /// <summary>
    /// SubscriptionTab.xaml 的交互逻辑
    /// </summary>
    public partial class SubscriptionTab : UserControl
    {
        MainWindow Main;

        public SubscriptionTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }
    }
}
