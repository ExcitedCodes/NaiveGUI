using System;
using System.Collections.Generic;

using NaiveGUI.Model;

namespace NaiveGUI.Data
{
    public class RemoteConfig : ModelBase
    {
        public static string[] ParseExtraHeaders(object original)
        {
            if (original == null)
            {
                return null;
            }
            if (original is string[] lol)
            {
                return lol;
            }
            var result = new List<string>();
            if (original is List<dynamic> lobj)
            {
                foreach (var i in lobj)
                {
                    if (!(i is string s))
                    {
                        throw new Exception("Unsupported parsing of extra headers");
                    }
                    result.Add(s);
                }
            }
            else if (original is string str)
            {
                foreach (var line in str.Replace('\r', '\n').Split('\n'))
                {
                    var trimmed = line.Trim(' ', '　', ' ', '\r', '\n');
                    if (trimmed != "")
                    {
                        result.Add(trimmed);
                    }
                }
            }
            else
            {
                throw new Exception("Unsupported parsing of extra headers");
            }
            return result.Count == 0 ? null : result.ToArray();
        }

        /// <summary>
        /// <see cref="Name"/> 和 <see cref="Group"/> 构成唯一索引且均不可为Null
        /// </summary>
        public string Name { get => _name; set => Set(out _name, value); }
        private string _name;

        public RemoteConfigGroup Group { get; set; }

        public ProxyType Type { get; set; } = ProxyType.Unknown;

        /// <summary>
        /// Extra headers split by CRLF, each element contains one header
        /// Pass --extra_headers only when this value is a string array and element count > 0
        /// </summary>
        public string[] ExtraHeaders { get; set; } = null;

        public bool Selected { get => _selected; set => Set(out _selected, value); }
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
