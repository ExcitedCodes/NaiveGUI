using System;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Globalization;

using NaiveGUI.Data;

namespace NaiveGUI.Model
{
    public class ListenerModel : ModelBase, IListener
    {
        public static string NaivePath = "naive.exe";
        public static IdnMapping IDN = new IdnMapping();

        public static Uri FilterListeningAddress(ref string input)
        {
            var builder = new UriBuilder(input);
            switch (builder.Scheme)
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

        public readonly MainViewModel Model;

        public bool IsReal => true;
        public ListenerModel Real => this;

        #region WPF Properties

        public bool Selected { get => _selected; set => Set(out _selected, value); }
        private bool _selected = false;

        public string SchemeUpper => Listen.Scheme.ToUpper();

        [SourceBinding(nameof(Enabled), nameof(Running), nameof(Started))]
        public string StatusText => Enabled ? (Started ? (Running ? "Active" : "Error") : "Pending") : "Disabled";

        [SourceBinding(nameof(Enabled), nameof(Running), nameof(Started))]
        public Brush StatusColor => (Brush)(Enabled && Started ? (Running ? App.Instance.Resources["ListenerColor_Active"] : App.Instance.Resources["ListenerColor_Error"]) : App.Instance.Resources["ListenerColor_Disabled"]);

        #endregion

        #region Listener Logic

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (value)
                {
                    if (Remote == null)
                    {
                        MessageBox.Show(MainViewModel.GetLocalized("Message_NoRemote"), "Oops", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    Started = false;
                    FailCounter = 0;
                }
                else
                {
                    FailCounter = 0;
                    WaitTick = 0;
                }
                Set(out _enabled, value);
            }
        }
        private bool _enabled;

        public bool Started { get => _started; set => Set(out _started, value); }
        private bool _started = false;

        public bool Running => BaseProcess != null && !BaseProcess.HasExited;

        public int WaitTick = 0, FailCounter = 0;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public Uri Listen
        {
            get => _listen;
            set
            {
                if (_listen == value)
                {
                    return;
                }
                _listen = value;
                Set(out _listen, value);
                if (Running)
                {
                    Stop();
                }
            }
        }
        private Uri _listen = null;

        public RemoteModel Remote
        {
            get => _remote;
            set
            {
                if (_remote == value)
                {
                    return;
                }
                Set(out _remote, value);
                if (Running)
                {
                    Stop();
                }
            }
        }
        private RemoteModel _remote = null;

        public Process BaseProcess = null;

        public ListenerModel(MainViewModel model, string listen, bool enabled = false) : this(model, FilterListeningAddress(ref listen), enabled) { }

        public ListenerModel(MainViewModel model, Uri listen, bool enabled = false)
        {
            Model = model;
            Listen = listen;
            Enabled = enabled;
        }

        public void HandleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Model.LogListener(Listen.ToString(), e.Data);
            }
        }

        public void Tick()
        {
            if (WaitTick > 0)
            {
                WaitTick--;
                return;
            }
            if (Enabled)
            {
                if (!Running && (Started || !Start()))
                {
                    Started = false;
                    if (++FailCounter > 3)
                    {
                        Enabled = false;
                        FailCounter = 0;
                        MainWindow.Instance.BalloonTip(Listen.ToString(), MainViewModel.GetLocalized("Tray_CrashedTooMany"));
                        MainWindow.Instance.Model.Save();
                        return;
                    }
                    WaitTick = 5 * 5;
                    MainWindow.Instance.BalloonTip(Listen.ToString(), MainViewModel.GetLocalized("Tray_Crashed"));
                }
            }
            else if (Running)
            {
                Stop();
            }
        }

        public bool Start()
        {
            if (Running)
            {
                return false;
            }

            // Escape IDN to punycode
            var builder = new UriBuilder(Remote.Remote.Uri);
            builder.Host = IDN.GetAscii(builder.Host);

            // Build start parameter
            var sb = new StringBuilder();
            sb.Append("--log=\"\"")
                .Append(" --listen=").Append(Listen.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped))
                .Append(" --proxy=").Append(builder.Uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped));
            if (Remote.ExtraHeaders != null && Remote.ExtraHeaders.Length != 0)
            {
                sb.Append(" --extra-headers=").Append(string.Join("\r\n", Remote.ExtraHeaders));
            }

            // Start naive.exe
            try
            {
                BaseProcess = Process.Start(new ProcessStartInfo(NaivePath, sb.ToString())
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                });

                BaseProcess.Exited += (s, e) => RaisePropertyChanged(nameof(Running));
                BaseProcess.EnableRaisingEvents = true;

                BaseProcess.OutputDataReceived += HandleOutput;
                BaseProcess.BeginOutputReadLine();
                BaseProcess.ErrorDataReceived += HandleOutput;
                BaseProcess.BeginErrorReadLine();

                Started = true;
                WaitTick = 5 * 2;
            }
            catch (Exception e)
            {
                // TODO: Log this error
                return false;
            }

            RaisePropertyChanged(nameof(Running));
            return true;
        }

        public void Stop()
        {
            try
            {
                if (!Running)
                {
                    return;
                }
                if (!BaseProcess.HasExited && !BaseProcess.CloseMainWindow())
                {
                    BaseProcess.Kill();
                }
            }
            catch { }
            finally
            {
                Started = false;
                FailCounter = 0;
                if (BaseProcess != null)
                {
                    BaseProcess.Dispose();
                    BaseProcess = null;
                }
            }
        }

        #endregion
    }
}
