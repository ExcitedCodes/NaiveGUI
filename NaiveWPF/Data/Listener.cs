using System;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;

using NaiveGUI.Helper;

namespace NaiveGUI.Data
{
    public class Listener : ModelBase, IListener
    {
        public static string NaivePath = "naive.exe";

        public static Uri FilterListeningAddress(ref string input)
        {
            var builder = new UriBuilder(input);
            switch(builder.Scheme)
            {
            case "socks":
            case "http":
            case "redir":
                break;
            default:
                builder.Scheme = "socks";
                break;
            }
            input = builder.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped);
            return builder.Uri;
        }

        public static void LogOutput(object sender, DataReceivedEventArgs e)
        {
            if(e.Data != null)
            {
                MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.Log(e.Data.Replace("\r", "").Replace("\n", "")));
            }
        }

        public bool IsReal => true;
        public Listener Real => this;

        public bool Selected => MainWindow.Instance.CurrentListener == this;

        public string StatusText => Enabled ? (Running ? "Active" : "Error") : "Disabled";
        public Brush StatusColor => (Brush)(Enabled ? (Running ? App.Instance.Resources["ListenerColor_Active"] : App.Instance.Resources["ListenerColor_Error"]) : App.Instance.Resources["ListenerColor_Disabled"]);

        public ProxyType Type { get; set; } = ProxyType.Unknown;

        /// <summary>
        /// 设置此属性请使用 <see cref="ToggleEnabled"/>
        /// </summary>
        public virtual bool Enabled
        {
            get => _enabled;
            private set
            {
                _enabled = value;
                RaisePropertyChanged("Enabled");
                RaisePropertyChanged("StatusText");
                RaisePropertyChanged("StatusColor");
            }
        }
        private bool _enabled;

        public virtual bool Running => BaseProcess != null && !BaseProcess.HasExited;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public virtual Uri Listen
        {
            get => _listen;
            set
            {
                if(Running && Listen != value)
                {
                    _listen = value;
                    Start();
                }
                else
                {
                    _listen = value;
                }
            }
        }
        private Uri _listen = null;

        public virtual string SchemeUpper => Listen.Scheme.ToUpper();

        public RemoteConfig Remote
        {
            get => _remote;
            set
            {
                if(_remote == value)
                {
                    return;
                }
                _remote = value;
                if(Running)
                {
                    Start();
                }
                RaisePropertyChanged("Remote");
            }
        }
        private RemoteConfig _remote = null;

        public Process BaseProcess = null;

        public int FailCounter = 0;
        public DateTime LastStart = DateTime.Now;

        public Listener(string listen, ProxyType type, bool enabled = false) : this(FilterListeningAddress(ref listen), type, enabled) { }

        public Listener(Uri listen, ProxyType type, bool enabled = false)
        {
            Type = type;
            Listen = listen;
            Enabled = enabled;
        }

        public void Tick(ulong Tick)
        {
            if(Enabled && Tick % 5 == 0)
            {
                if(!Running)
                {
                    if(DateTime.Now - LastStart < TimeSpan.FromSeconds(10))
                    {
                        if(++FailCounter > 3)
                        {
                            Enabled = false;
                            MainWindow.Instance.BalloonTip(Listen.ToString(), MainWindow.GetLocalized("Tray_CrashedTooMany"));
                            MainWindow.Instance.Save();
                            return;
                        }
                        MainWindow.Instance.BalloonTip(Listen.ToString(), MainWindow.GetLocalized("Tray_Crashed"));
                    }
                    Start();
                }
                else if(FailCounter != 0 && DateTime.Now - LastStart > TimeSpan.FromSeconds(30))
                {
                    FailCounter = 0;
                }
            }
        }

        public bool ToggleEnabled()
        {
            if(Remote == null)
            {
                MessageBox.Show(MainWindow.GetLocalized("Message_NoRemote"), "Oops", MessageBoxButton.OK, MessageBoxImage.Warning);
                return Enabled = false;
            }
            Enabled = !Enabled;
            MainWindow.Instance.Save();
            if(Enabled)
            {
                FailCounter = 0;
                Start();
            }
            else
            {
                Stop();
            }
            return Enabled;
        }

        /// <summary>
        /// Will restart if already running
        /// </summary>
        public void Start()
        {
            Stop();
            var sb = new StringBuilder();
            sb.Append("--listen=").Append(Listen.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped))
                .Append(" --proxy=").Append(Remote.Remote.Uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped));
            if (Remote.ExtraHeaders != null && Remote.ExtraHeaders.Length != 0)
            {
                sb.Append(" --extra-headers=").Append(string.Join("\r\n", Remote.ExtraHeaders));
            }
            // TODO: --host-resolver-rules=
            bool logging = MainWindow.Instance.Logging.Value;
            if(logging)
            {
                sb.Append(" --log=\"\"");
            }
            // TODO: --log-net-log, --ssl-key-log-file
            var start = new ProcessStartInfo(NaivePath, sb.ToString())
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = !logging,
                RedirectStandardError = logging,
                RedirectStandardOutput = logging
            };
            LastStart = DateTime.Now;
            try
            {
                BaseProcess = Process.Start(start);
                BaseProcess.Exited += (s, e) =>
                {
                    RaisePropertyChanged("StatusText");
                    RaisePropertyChanged("StatusColor");
                };
                if(logging)
                {
                    BaseProcess.OutputDataReceived += LogOutput;
                    BaseProcess.BeginOutputReadLine();
                    BaseProcess.ErrorDataReceived += LogOutput;
                    BaseProcess.BeginErrorReadLine();
                }
            }
            catch { }
            RaisePropertyChanged("StatusText");
            RaisePropertyChanged("StatusColor");
        }

        public void Stop()
        {
            if(BaseProcess == null)
            {
                return;
            }
            try
            {
                if(!BaseProcess.HasExited && !BaseProcess.CloseMainWindow())
                {
                    BaseProcess.Kill();
                }
                BaseProcess.Dispose();
            }
            catch { }
            BaseProcess = null;
        }
    }
}
