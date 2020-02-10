using System;
using System.Text;
using System.Diagnostics;

namespace NaiveGUI.Data
{
    public class Listener : IListener
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
                // MainForm.Instance.Invoke(new Action(() => MainForm.Instance.textBox_log.AppendText(e.Data.Replace("\r", "").Replace("\n", "") + Environment.NewLine)));
            }
        }

        public Listener Real => this;

        /// <summary>
        /// 设置此属性请使用 <see cref="ToggleEnabled"/>
        /// </summary>
        public virtual bool Enabled { get; private set; }

        public virtual bool Running => BaseProcess != null && !BaseProcess.HasExited;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public virtual Uri Listen
        {
            get => this._listen;
            set
            {
                this._listen = value;
                if(Running)
                {
                    // Start();
                }
            }
        }
        private Uri _listen = null;

        public virtual string SchemeUpper => Listen.Scheme.ToUpper();

        public RemoteConfig Remote
        {
            get => this._remote;
            set
            {
                this._remote = value;
                if(Running)
                {
                    // Start();
                }
            }
        }

        private RemoteConfig _remote;

        public Process BaseProcess = null;

        public int FailCounter = 0;
        public DateTime LastStart = DateTime.Now;
        /*
        public Listener(string listen, bool enabled = false) : this(FilterListeningAddress(ref listen), enabled) { }

        public Listener(Uri listen, bool enabled = false)
        {
            Listen = listen;
            Enabled = enabled;
        }

        public void Tick(ulong Tick)
        {
            if(Enabled && Tick % 20 == 0)
            {
                if(!Running)
                {
                    if(DateTime.Now - LastStart < TimeSpan.FromSeconds(10))
                    {
                        if(++FailCounter > 3)
                        {
                            Enabled = false;
                            MainForm.Instance.BalloonTip(Listen.ToString(), "Listener crashed for too many times, manually maintenance required.", ToolTipIcon.Error);
                            MainForm.Instance.RefreshListenerList();
                            MainForm.Instance.Save();
                            return;
                        }
                        MainForm.Instance.BalloonTip(Listen.ToString(), "Listener crashed, restarting...", ToolTipIcon.Error);
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
               // MessageBox.Show("You must select a remote before starting listener! Tick the checkbox in remote list to select it.", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return Enabled = false;
            }
            Enabled = !Enabled;
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
            if(Remote.Padding)
            {
                sb.Append(" --padding");
            }
            // TODO: --host-resolver-rules=
            bool logging = MainForm.Instance.checkBox_logging.Checked;
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
                if(logging)
                {
                    BaseProcess.OutputDataReceived += LogOutput;
                    BaseProcess.BeginOutputReadLine();
                    BaseProcess.ErrorDataReceived += LogOutput;
                    BaseProcess.BeginErrorReadLine();
                }
            }
            catch { }
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
        }*/
    }
}
