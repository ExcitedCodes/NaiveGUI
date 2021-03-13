using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using NaiveGUI.Model;

namespace NaiveGUI.View
{
    /// <summary>
    /// LogTab.xaml 的交互逻辑
    /// </summary>
    public partial class LogTab : UserControl
    {
        private readonly MainViewModel Model;

        private bool AutoScroll = true;

        public LogTab(MainViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Log File|*.log|All Files|*.*",
                FileName = "NaiveGUI_" + DateTime.Now.ToString("yyyy-MM-dd"),
                DefaultExt = ".log"
            };
            if (dialog.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                foreach (var l in Model.Logs)
                {
                    sb.Append(l.Time).Append(' ')
                        .Append(l.Source).Append(' ')
                        .Append(l.Level).Append(' ')
                        .Append(l.Data).Append('\n');
                }
                File.WriteAllText(dialog.FileName, sb.ToString());
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e) => Model.Logs.Clear();

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.Source is ScrollViewer sv)
            {
                if (e.ExtentHeightChange == 0)
                {
                    AutoScroll = sv.VerticalOffset == sv.ScrollableHeight;
                }
                else if (AutoScroll)
                {
                    sv.ScrollToVerticalOffset(sv.ExtentHeight);
                }
            }
        }
    }
}
