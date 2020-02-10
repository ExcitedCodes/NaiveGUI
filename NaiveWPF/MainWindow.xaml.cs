using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

using NaiveGUI.View;
using NaiveGUI.Data;

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

        public UserControl[] Tabs = null;
        public Prop<int> CurrentTab { get; set; } = new Prop<int>();

        public ObservableCollection<IListener> Listeners { get; set; } = new ObservableCollection<IListener>();
        public ObservableCollection<RemoteConfigGroup> Remotes { get; set; } = new ObservableCollection<RemoteConfigGroup>();

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
            InitializeComponent();
            DataContext = this;
            
            Listeners.Add(new AddListenerButton());

            var g = new RemoteConfigGroup("GroupA")
            {
                new RemoteConfig("XD")
            };
            Remotes.Add(g);

            SwitchTab(0);
        }

        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            ReleaseCapture();
            SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        #region Tab Switching

        public void SwitchTab(int id)
        {
            CurrentTab.Value = id;
            tabContents.BeginStoryboard(Resources["TabHideAnimation"] as Storyboard);
        }

        private void ButtonTab_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag as string);
            if(CurrentTab.Value != id)
            {
                SwitchTab(id);
                RaisePropertyChanged("CurrentTabTester");
            }
        }

        private void StoryboardTabHideAnimation_Completed(object sender, EventArgs e)
        {
            tabContents.Child = Tabs[CurrentTab.Value];
            tabContents.BeginStoryboard(Resources["TabShowAnimation"] as Storyboard);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
