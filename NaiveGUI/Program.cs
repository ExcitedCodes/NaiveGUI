using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Principal;

namespace NaiveGUI
{
    static class Program
    {
        public static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public static string AutoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\NaiveGUI_" + Utils.Md5(Application.ExecutablePath) + ".lnk";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool minimize = false;
            var config = "config.json";
            foreach(var a in args)
            {
                var split = a.Split('=');
                if(split.Length == 2 && (split[0] == "--autorun"))
                {
                    // Restricted to the binary itself, sometimes parent is x64 process and there's an exception
                    try
                    {
                        var parent = ParentProcessFinder.GetParentProcess(Process.GetCurrentProcess().Handle);
                        if(parent != null && parent.Modules[0].FileName == Application.ExecutablePath)
                        {
                            SetAutoRun(bool.TryParse(split[1], out bool start) && start);
                        }
                    }
                    catch { }
                    return;
                }
                if(split.Length == 2 && (split[0] == "-c" || split[0] == "--config"))
                {
                    config = split[1];
                }
                else if(split[0] == "--minimize")
                {
                    minimize = true;
                }
            }
            var full = Path.GetFullPath(config);
            AutoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\NaiveGUI_" + Utils.Md5(Application.ExecutablePath + full) + ".lnk";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mutex = new Mutex(true, "NaiveGUI_" + Utils.Md5(full), out bool created);
            if(created)
            {
                Application.Run(new MainForm(full, minimize, File.Exists(AutoRunFile)));
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

        public static bool SetAutoRun(bool start)
        {
            try
            {
                if(start)
                {
                    if(File.Exists(AutoRunFile))
                    {
                        return true;
                    }
                    // Don't use using here, IWshRuntimeLibrary.File will cause name conflict.
                    var shortcut = (IWshRuntimeLibrary.IWshShortcut)new IWshRuntimeLibrary.WshShell().CreateShortcut(AutoRunFile);
                    shortcut.TargetPath = Application.ExecutablePath;
                    shortcut.Arguments = "--minimize --config=\"" + MainForm.Instance.ConfigPath.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                    shortcut.WorkingDirectory = Application.StartupPath;
                    shortcut.Description = "Naive Proxy GUI Auto Start";
                    shortcut.Save();
                }
                else if(File.Exists(AutoRunFile))
                {
                    File.Delete(AutoRunFile);
                }
                return true;
            }
            catch(Exception e)
            {
                if(IsAdministrator)
                {
                    MessageBox.Show("Failed to set autostart status even I have administrator privilege.\nYou may wanted to check your anti-virus software.\n\n" + e.ToString(), "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(MessageBox.Show("Failed to set autostart status.\nGenerally this shouldn't happen, do you want to try again as administrator?\nAlso make sure your anti-virus software isn't blocking this progress.", "Oops", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(Application.ExecutablePath)
                        {
                            Verb = "runas",
                            Arguments = "--autorun=" + start.ToString(),
                            UseShellExecute = true,
                            WorkingDirectory = Environment.CurrentDirectory
                        }).WaitForExit();
                        return true;
                    }
                    catch
                    {
                        MessageBox.Show("Unable to elevate the process.", "WTF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return false;
        }
    }
}
