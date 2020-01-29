using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace NaiveGUI
{
    public class ProxyListener
    {
        public static string NaivePath = "naive.exe";

        /// <summary>
        /// 设置此属性请使用 <see cref="ToggleEnabled"/>
        /// </summary>
        public bool Enabled { get; private set; }

        public bool Running => BaseProcess != null && !BaseProcess.HasExited;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public UriBuilder Listen = null;

        public RemoteConfig Remote
        {
            get => this._remote;
            set
            {
                this._remote = value;
                if(Running)
                {
                    // TODO: This may trigger fail check, lock required
                    Stop();
                    Start();
                }
            }
        }
        private RemoteConfig _remote;

        public Process BaseProcess = null;

        public int FailCounter = 0;
        public DateTime LastStart = DateTime.Now;

        public ProxyListener(UriBuilder listen, bool enabled = false)
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
                MessageBox.Show("You must select a remote before starting listener! Tick the checkbox in remote list to select it.", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        public void Start()
        {
            Stop();
            var sb = new StringBuilder();
            sb.Append("--listen=").Append(Listen.Uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped))
                .Append(" --proxy=").Append(Remote.Remote.Uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.SafeUnescaped));
            if(Remote.Padding)
            {
                sb.Append(" --padding");
            }
            // TODO: --host-resolver-rules=
            if(Remote.QuicVersion > 0)
            {
                sb.Append(" --quic-version=").Append(Remote.QuicVersion);
            }
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
                    BaseProcess.OutputDataReceived += (s, e) =>
                    {
                        if(e.Data != null)
                            MainForm.Instance.Invoke(new Action(() =>
                            {
                                MainForm.Instance.textBox_log.AppendText(e.Data);
                            }));
                    };
                    BaseProcess.BeginOutputReadLine();
                    BaseProcess.ErrorDataReceived += (s, e) =>
                    {
                        if(e.Data != null)
                            MainForm.Instance.Invoke(new Action(() =>
                            {
                                MainForm.Instance.textBox_log.AppendText(e.Data);
                            }));
                    };
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
        }
    }
}
