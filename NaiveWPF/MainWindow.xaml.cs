using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Runtime.InteropServices;

namespace NaiveGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static MainWindow Instance = null;

        public StringBuilder LogBuffer = new StringBuilder();

        public UserControl[] Tabs = null;
        public Prop<int> CurrentTab { get; set; } = new Prop<int>();

        public TabIndexTester CurrentTabTester { get; set; }

        public MainWindow()
        {
            Instance = this;
            Tabs = new UserControl[] {
                new ProxyTab(this),
                new SubscriptionTab(this),
                new LogTab(this),
                new SettingsTab(this)
            };
            CurrentTabTester = new TabIndexTester(this);
            DataContext = this;
            InitializeComponent();

            SwitchTab(0);
        }

        public void Log(string data, string newline = "\n")
        {
            LogBuffer.Append(data).Append(newline);
            RaisePropertyChanged("LogBuffer");
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

        private void ButtonTab_Click(object sender, RoutedEventArgs e)
        {
            SwitchTab(int.Parse((sender as Button).Tag as string));
            RaisePropertyChanged("CurrentTabTester");
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion

        public class TabIndexTester
        {
            public MainWindow Main;

            public bool this[int offset] => Main.CurrentTab == offset;

            public TabIndexTester(MainWindow main)
            {
                Main = main;
            }
        }
    }
}
