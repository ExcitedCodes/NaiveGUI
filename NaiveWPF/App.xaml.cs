using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using NaiveGUI.Helper;

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

        [DllImport("kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);

        public static readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
        public static readonly bool IsAdministrator = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public static string AutoRunFile { get; private set; }
        public static string DefaultUserAgent = "NaiveGUI/" + Assembly.GetExecutingAssembly().GetName().Version + " (Potato NT) not AppleWebKit (not KHTML, not like Gecko) not Chrome not Safari";

        public static App Instance = null;

        #region Assistant Methods

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

        public static string HttpGetString(string url, Encoding encoding = null, int timeoutMs = 5000, bool redirect = false, IWebProxy proxy = null)
        {
            if(encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(HttpGetBytes(url, timeoutMs, redirect, proxy));
        }

        public static byte[] HttpGetBytes(string url, int timeoutMs = -1, bool redirect = false, IWebProxy proxy = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if(url.StartsWith("//"))
            {
                url = "https:" + url;
            }
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.AllowAutoRedirect = redirect;
            if(proxy != null)
            {
                request.Proxy = proxy;
            }
            if(timeoutMs > 0)
            {
                request.Timeout = timeoutMs;
            }
            using(var response = request.GetResponse() as HttpWebResponse)
            {
                if(response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Bad HTTP Status(" + url + "):" + response.StatusCode + " " + response.StatusDescription);
                }
                using(var ms = new MemoryStream())
                {
                    response.GetResponseStream().CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static string Md5(byte[] data)
        {
            try
            {
                StringBuilder Result = new StringBuilder();
                foreach(byte Temp in new MD5CryptoServiceProvider().ComputeHash(data))
                {
                    if(Temp < 16)
                    {
                        Result.Append("0");
                        Result.Append(Temp.ToString("x"));
                    }
                    else
                    {
                        Result.Append(Temp.ToString("x"));
                    }
                }
                return Result.ToString();
            }
            catch
            {
                return "0000000000000000";
            }
        }

        public static string Md5(string Data) => Md5(EncodeByteArray(Data));

        public static byte[] EncodeByteArray(string data) => data == null ? null : Encoding.UTF8.GetBytes(data);

        #endregion

        #region WTF

        public static string YAAYYYYYAAAAAAAAAAYYYYYYYYYYVBYAAAAAAAAAAAY(string message = "", string data = "", string title = "YAAAAAAAAAAAAAAAAAAAAAY") => Microsoft.VisualBasic.Interaction.InputBox(message, title, data);

        #endregion

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
            AppMutex = new Mutex(true, "NaiveGUI_" + Md5(full), out bool created);
            AutoRunFile = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\NaiveGUI_" + Md5(ExecutablePath + full) + ".lnk";
            if(created)
            {
                MainWindow = new MainWindow(config, File.Exists(AutoRunFile));
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

        private void TrayMenu_Exit(object sender, RoutedEventArgs e) => MainWindow.Close();
    }
}
