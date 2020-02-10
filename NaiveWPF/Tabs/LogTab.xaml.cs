using System.Windows.Controls;

namespace NaiveGUI
{
    /// <summary>
    /// LogTab.xaml 的交互逻辑
    /// </summary>
    public partial class LogTab : UserControl
    {
        MainWindow Main;

        public LogTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }
    }
}
