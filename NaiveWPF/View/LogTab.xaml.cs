using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text.RegularExpressions;

using Microsoft.Win32;

using NaiveGUI.Model;

namespace NaiveGUI.View
{
    /// <summary>
    /// LogTab.xaml 的交互逻辑
    /// </summary>
    public partial class LogTab : UserControl
    {
        public static Regex LogPattern = new Regex(@"\[(?<Time>\d{4}/[\d\.]+)\:(?<Level>[A-Z]+)\:(?<Source>[a-zA-Z0-9_\.\(\)]+)\] (?<Content>.+)", RegexOptions.Compiled | RegexOptions.Singleline);

        public static readonly SolidColorBrush BrushInfo = new SolidColorBrush(Colors.White),
             BrushWarning = new SolidColorBrush(Colors.Orange),
             BrushError = new SolidColorBrush(Color.FromRgb(220, 80, 54)),
             BrushTime = new SolidColorBrush(Color.FromRgb(80, 141, 220)),
             BrushText = new SolidColorBrush(Colors.Silver);

        private readonly MainViewModel Model;

        public LogTab(MainViewModel model)
        {
            InitializeComponent();
            DataContext = Model = model;
        }

        public void Log(string raw)
        {
            bool bottom = ScrollViewerLog.ScrollableHeight - ScrollViewerLog.VerticalOffset < 1;
            if (TextBlockLog.Inlines.Count != 0)
            {
                AddLineBreak();
            }
            var match = LogPattern.Match(raw);
            if (!match.Success)
            {
                AddRun(raw, BrushText);
            }
            else
            {
                AddRun(match.Groups["Time"].Value + " ", BrushTime);
                var levelColor = BrushInfo;
                switch (match.Groups["Level"].Value)
                {
                case "WARNING":
                    levelColor = BrushWarning;
                    break;
                case "ERROR":
                    levelColor = BrushError;
                    break;
                }
                AddRun(match.Groups["Level"].Value + ":" + match.Groups["Source"].Value + " ", levelColor);
                AddRun(match.Groups["Content"].Value, BrushText);
            }
            // 4 inlines/row, we also need to remove the 1st linebreak
            while (TextBlockLog.Inlines.Count > 400 - 1)
            {
                TextBlockLog.Inlines.Remove(TextBlockLog.Inlines.FirstInline);
            }
            if (bottom)
            {
                ScrollViewerLog.ScrollToBottom();
            }
        }

        public void AddRun(string text, Brush color) => TextBlockLog.Inlines.Add(new Run(text)
        {
            Foreground = color
        });

        public void AddLineBreak() => TextBlockLog.Inlines.Add(new LineBreak());

        private void ButtonSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Log File|*.log|All Files|*.*",
                FileName = "NaiveGUI_" + DateTime.Now.ToString("yyyy-MM-dd"),
                DefaultExt = ".log"
            };
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, TextBlockLog.Text);
            }
        }

        private void ButtonClear_Click(object sender, System.Windows.RoutedEventArgs e) => TextBlockLog.Inlines.Clear();
    }
}
