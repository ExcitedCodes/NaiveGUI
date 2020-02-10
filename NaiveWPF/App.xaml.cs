using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace NaiveGUI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        public static readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
        public static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public static string AutoRunFile { get; private set; }

        public static App Instance = null;

        public Mutex AppMutex = null;

        public App() : base()
        {
            Instance = this;
        }

        private void Application_Exit(object sender, ExitEventArgs e) => AppMutex?.ReleaseMutex();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var config = "config.json";
            var minimize = false;
            foreach(var a in e.Args)
            {
                var split = a.Split('=');
                if(split.Length == 2 && (split[0] == "--autorun"))
                {
                    // Restricted to the binary itself, sometimes parent is x64 process and there's an exception
                    try
                    {
                        var parent = ParentProcessFinder.GetParentProcess(Process.GetCurrentProcess().Handle);
                        if(parent != null && parent.Modules[0].FileName == ExecutablePath)
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
            AppMutex = new Mutex(true, "NaiveGUI_" + Utils.Md5(full), out bool created);
            AutoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\NaiveGUI_" + Utils.Md5(ExecutablePath + full) + ".lnk";
            if(created)
            {
                MainWindow = new MainWindow(config, false);
                if(!minimize)
                {
                    MainWindow.Show();
                }
            }
            else
            {
                MessageBox.Show("There's already an NaiveGUI instance running, please look for its tray icon.\n" +
                    "If you want to run multiple instances, please copy the program to a different path or use --config/-c=yourconfig.json to specify config path.\n" +
                    "\n" +
                    "Config: " + config + "\n" +
                    "Full Path: " + full, "Oops", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    // Don't include IWshRuntimeLibrary here, IWshRuntimeLibrary.File will cause name conflict.
                    var shortcut = (IWshRuntimeLibrary.IWshShortcut)new IWshRuntimeLibrary.WshShell().CreateShortcut(AutoRunFile);
                    shortcut.TargetPath = ExecutablePath;
                    shortcut.Arguments = "--minimize --config=\"" + (Instance.MainWindow as MainWindow).ConfigPath.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                    shortcut.Description = "Naive Proxy GUI Auto Start";
                    shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
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
                    MessageBox.Show("Failed to set autostart status even I have administrator privilege.\nYou may wanted to check your anti-virus software.\n\n" + e.ToString(), "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if(MessageBox.Show("Failed to set autostart status.\nGenerally this shouldn't happen, do you want to try again as administrator?\nAlso make sure your anti-virus software isn't blocking this progress.", "Oops", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(ExecutablePath)
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
                        MessageBox.Show("Unable to elevate the process.", "WTF", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return false;
        }
    }
}
