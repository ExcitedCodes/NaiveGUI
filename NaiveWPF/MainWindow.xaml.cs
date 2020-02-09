using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace NaiveGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public Prop<int> CurrentTab { get; set; } = new Prop<int>();

        public UserControl[] Tabs = new UserControl[] {
            new ProxyTab(),
            new SubscriptionTab(),
            new LogTab(),
            new SettingsTab()
        };
        
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            SwitchTab(0);
        }

        public void SwitchTab(int id)
        {
            CurrentTab.Value = id;
            tabContents.Content = Tabs[id];
        }
        
        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
        }
        
        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        private void ButtonTab_Click(object sender, RoutedEventArgs e) => SwitchTab(int.Parse((string)((FrameworkElement)sender).Tag));
    }
}
