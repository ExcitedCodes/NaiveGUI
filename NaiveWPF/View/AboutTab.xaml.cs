using System.Reflection;
using System.Windows.Controls;

namespace NaiveGUI.View
{
    /// <summary>
    /// AboutTab.xaml 的交互逻辑
    /// </summary>
    public partial class AboutTab : UserControl
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public string BuildTime => "(" + Properties.Resources.BuildDate.Replace("\r", "").Replace("\n", "").Trim() + ")";
        public string License => Properties.Resources.LICENSE;

        public AboutTab()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
