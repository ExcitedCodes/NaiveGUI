using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace NaiveGUI
{
    public class ProxyListener
    {
        public static string NaivePath = "naive.exe";

        public bool Enabled = false;

        public bool Running => BaseProcess != null && !BaseProcess.HasExited;

        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public UriBuilder Listen = null;

        public Process BaseProcess = null;

        /// <summary>
        /// 设置此属性请使用 <see cref="SetRemote"/>
        /// </summary>
        public RemoteConfig Remote
        {
            get => this._remote;
            set
            {
                this._remote = value;
                if(Running)
                {
                    BaseProcess.Exited -= process_Exited;
                    Stop();
                    Start();
                }
            }
        }
        private RemoteConfig _remote;

        public int FailCounter = 0;

        public ProxyListener(UriBuilder listen)
        {
            Listen = listen;
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
                Start();
            }
            else
            {
                Stop();
            }
            return Enabled;
        }

        public void Tick()
        {

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
            // TODO: --log, --log-net-log, --ssl-key-log-file
            var start = new ProcessStartInfo(NaivePath, sb.ToString())
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = !logging,
                RedirectStandardError = logging,
                RedirectStandardOutput = logging
            };
            BaseProcess = Process.Start(start);
            BaseProcess.Exited += process_Exited;
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

        public void Stop()
        {
            if(BaseProcess == null)
            {
                return;
            }
            try
            {
                FailCounter = -1;
                if(!BaseProcess.HasExited && !BaseProcess.CloseMainWindow())
                {
                    BaseProcess.Kill();
                }
                BaseProcess.Dispose();
            }
            catch { }
            BaseProcess = null;
        }

        protected void process_Exited(object sender, EventArgs args)
        {

        }
    }
}
