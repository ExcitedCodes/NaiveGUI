using System;
using System.Text;
using System.Diagnostics;

namespace NaiveGUI
{
    public class ProxyListener
    {
        public static string NaivePath = "naive.exe";

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public UriBuilder Listen = null;

        public Process BaseProcess = null;
        public RemoteConfig Remote = null;

        public bool Working => BaseProcess == null ? false : BaseProcess.HasExited;

        public ProxyListener()
        {

        }

        public void Start()
        {
            Stop();
            var sb = new StringBuilder();
            sb.Append("--listen=").Append(Listen.ToString())
                .Append(" --proxy=").Append(Remote.Remote.ToString());
            if(Remote.Padding)
            {
                sb.Append(" --padding");
            }
            // TODO: --host-resolver-rules=
            if(Remote.QuicVersion > 0)
            {
                sb.Append(" --quic-version=").Append(Remote.QuicVersion);
            }
            sb.Append(" --log=\"\"");
            // TODO: --log, --log-net-log, --ssl-key-log-file
            var start = new ProcessStartInfo(NaivePath, sb.ToString())
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute=false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            BaseProcess = Process.Start(start);
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
