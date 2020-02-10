using System.Windows.Controls;

namespace NaiveGUI
{
    /// <summary>
    /// SettingsTab.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsTab : UserControl
    {
        MainWindow Main;

        public SettingsTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }
    }
}
