using System.Windows.Media;
using System.Text.RegularExpressions;

namespace NaiveGUI.Model
{
    public class LogModel
    {
        public static readonly string TimeFormat = "MM-dd HH:mm:ss";
        public static readonly Regex Pattern = new Regex(@"\[(?<Time>\d{4}/[\d\.]+)\:(?<Level>[A-Z]+)\:(?<Source>[a-zA-Z0-9_\.\(\)]+)\] (?<Content>.+)", RegexOptions.Compiled | RegexOptions.Singleline);

        public static readonly SolidColorBrush BrushWarning = new SolidColorBrush(Colors.Orange),
            BrushError = new SolidColorBrush(Color.FromRgb(220, 80, 54));

        public string Source { get; set; }
        public string Time { get; set; }
        public string Level { get; set; }
        public string Data { get; set; }

        public SolidColorBrush LevelColor { get; set; } = new SolidColorBrush(Colors.White);
    }
}
