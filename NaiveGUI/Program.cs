using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace NaiveGUI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var config = "config.json";
            foreach(var a in args)
            {
                var split = a.Split('=');
                if(split.Length == 2 && (split[0] == "-c" || split[0] == "--config"))
                {
                    config = split[1];
                }
            }
            var full = Path.GetFullPath(config);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mutex = new Mutex(true, "NaiveGUI_" + Utils.Md5(full), out bool created);
            if(created)
            {
                Application.Run(new MainForm(full));
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("There's already an NaiveGUI instance running, please look for its tray icon.\n" +
                    "If you want to run multiple instances, please copy the program to a different path or use --config/-c=yourconfig.json to specify config path.\n" +
                    "\n" +
                    "Config: " + config + "\n" +
                    "Full Path: " + full, "Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
