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

        private void ToggleButtonAutoRun_Checked(object sender, System.Windows.RoutedEventArgs e) => App.SetAutoRun(true);

        private void ToggleButtonAutoRun_Unchecked(object sender, System.Windows.RoutedEventArgs e) => App.SetAutoRun(false);
    }
}
