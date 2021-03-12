using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Media.Animation;

using MaterialDesignThemes.Wpf;
using Hardcodet.Wpf.TaskbarNotification;

using NaiveGUI.View;
using NaiveGUI.Model;

namespace NaiveGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly MainViewModel Model;

        public static MainWindow Instance;

        public UserControl[] Tabs = null;
        public SnackbarMessageQueue snackbarMessageQueue { get; set; } = new SnackbarMessageQueue();

        public MainWindow(string config)
        {
            Instance = this;

            InitializeComponent();
            DataContext = Model = new MainViewModel(this, config);

            Tabs = new UserControl[] {
                new ProxyTab(Model),
                new SubscriptionTab(Model),
                new LogTab(Model),
                new SettingsTab(Model),
                new AboutTab()
            };
        }

        public void Log(string raw) => (Tabs[2] as LogTab).Log(raw);

        #region General Events

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var l in Model.Listeners)
            {
                if (l.IsReal && l.Real.Running)
                {
                    l.Real.Stop();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.ReleaseCapture();
            App.SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => Model.Save();

        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        #endregion

        #region Tray Icon & Menu

        public void BalloonTip(string title, string text) => trayIcon.ShowBalloonTip(title, text, BalloonIcon.Error);

        private void TrayIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
            Topmost = true; // Still using this in 2020?
            Topmost = false;
        }

        #endregion

        #region Tab Switching

        public void BeginTabStoryboard(string name) => tabContents.BeginStoryboard(Resources[name] as Storyboard);

        private void ButtonTab_Click(object sender, RoutedEventArgs e) => Model.SwitchTab(int.Parse((sender as Button).Tag as string));

        private void StoryboardTabHideAnimation_Completed(object sender, EventArgs e)
        {
            tabContents.Child = Tabs[Model.CurrentTab];
            BeginTabStoryboard("TabShowAnimation");
        }

        #endregion
    }
}
