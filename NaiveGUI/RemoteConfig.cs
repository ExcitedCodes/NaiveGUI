using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveGUI
{
    public class RemoteConfig
    {
        /// <summary>
        /// 原则上 <see cref="Name"/> 和 <see cref="Group"/> 构成唯一索引且均不可为Null
        /// </summary>
        public string Name = null, Group = null;

        /// <summary>
        /// Obfuscates traffic by adding length paddings.
        /// </summary>
        public bool Padding = false;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public UriBuilder Remote = new UriBuilder("https://localhost")
        {
            Port = -1
        };

        /// <summary>
        /// Forces a QUIC version. Allowed values: 39, 43, 46-49, or later.
        /// </summary>
        public int QuicVersion = -1;

        public RemoteConfig(string group, string name)
        {
            Group = group;
            Name = name;
        }
    }
}
