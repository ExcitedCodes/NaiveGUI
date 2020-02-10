using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Text.RegularExpressions;

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

        MainWindow Main;

        public LogTab(MainWindow main)
        {
            InitializeComponent();
            DataContext = Main = main;
        }

        public void Log(string raw)
        {
            if(TextBlockLog.Inlines.Count != 0)
            {
                AddLineBreak();
            }
            var match = LogPattern.Match(raw);
            if(!match.Success)
            {
                AddRun(raw, BrushText);
            }
            else
            {
                AddRun(match.Groups["Time"].Value + " ", BrushTime);
                var levelColor = BrushInfo;
                switch(match.Groups["Level"].Value)
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
        }

        public void AddRun(string text, Brush color) => TextBlockLog.Inlines.Add(new Run(text)
        {
            Foreground = color
        });

        public void AddLineBreak() => TextBlockLog.Inlines.Add(new LineBreak());
    }
}
