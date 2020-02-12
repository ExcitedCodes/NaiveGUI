using System;

using NaiveGUI.Helper;

namespace NaiveGUI.Data
{
    public class RemoteConfig : ModelBase
    {
        /// <summary>
        /// <see cref="Name"/> 和 <see cref="Group"/> 构成唯一索引且均不可为Null
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
        private string _name;

        public RemoteConfigGroup Group { get; set; }

        public ProxyType Type { get; set; } = ProxyType.Unknown;

        /// <summary>
        /// Obfuscates traffic by adding length paddings.
        /// </summary>
        public bool Padding { get; set; } = false;

        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RaisePropertyChanged();
            }
        }
        private bool _selected;

        /// <summary>
        /// Listens at addr:port with protocol &lt;proto&gt;.
        /// Allowed values for proto: "socks", "http", "redir".
        /// </summary>
        public UriBuilder Remote { get; set; } = new UriBuilder("https://localhost")
        {
            Port = -1
        };

        public RemoteConfig(string name, ProxyType type)
        {
            Name = name;
            Type = type;
        }
    }
}
