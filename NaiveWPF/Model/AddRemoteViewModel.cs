using System;

using NaiveGUI.Data;

namespace NaiveGUI.Model
{
    public class AddRemoteViewModel : ModelBase
    {
        public readonly AddRemoteWindow View;

        public AddRemoteViewModel(AddRemoteWindow view, RemoteModel config = null)
        {
            View = view;
            Config = config;

            RemoteName = config.Name;
            RemoteURI = config.Remote.ToString();
            ExtraHeaders = config.ExtraHeaders == null ? "" : string.Join(Environment.NewLine, config.ExtraHeaders);
        }

        public AddRemoteViewModel(AddRemoteWindow view, RemoteGroupModel group, string name, string uri, string extra_headers)
        {
            View = view;
            Group = group;

            RemoteName = name ?? "";
            RemoteURI = uri ?? "";
            ExtraHeaders = extra_headers ?? "";
        }

        public RemoteModel Config = null;
        public RemoteGroupModel Group = null;

        public string RemoteName { get => _remoteName; set => Set(out _remoteName, value); }
        public string _remoteName = "";

        public string RemoteURI { get => _remoteURI; set => Set(out _remoteURI, value); }
        public string _remoteURI = "";

        public string ExtraHeaders { get => _extraHeaders; set => Set(out _extraHeaders, value); }
        public string _extraHeaders = "";

        public bool Commit()
        {
            if (RemoteName.Trim() == "" || RemoteURI.Trim() == "")
            {
                return false;
            }
            if (Config != null)
            {
                Config.Remote = new UriBuilder(RemoteURI);
                Config.ExtraHeaders = RemoteModel.ParseExtraHeaders(ExtraHeaders);
                if (RemoteName != Config.Name)
                {
                    var g = Config.Group;
                    for (int i = 0; i < g.Count; i++)
                    {
                        if (g[i].Name == RemoteName)
                        {
                            g.RemoveAt(i--);
                        }
                    }
                    Config.Name = RemoteName;
                }
            }
            else
            {
                for (int i = 0; i < Group.Count; i++)
                {
                    if (Group[i].Name == RemoteName)
                    {
                        Group.RemoveAt(i--);
                    }
                }
                Group.Add(new RemoteModel(RemoteName)
                {
                    Remote = new UriBuilder(RemoteURI),
                    ExtraHeaders = RemoteModel.ParseExtraHeaders(ExtraHeaders)
                });
            }
            MainWindow.Instance.Model.Save();
            return true;
        }
    }
}
