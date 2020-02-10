using System.Windows.Controls;

namespace NaiveGUI.View
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

        private void ToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ToggleButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
