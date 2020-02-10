using System;
using System.Collections.ObjectModel;

namespace NaiveGUI.Data
{
    public class DesignerMainWindow
    {
        public ObservableCollection<IListener> Listeners => new ObservableCollection<IListener>()
        {
            new DesignerListener()
            {
                Listen = new Uri("socks://0.0.0.0:1080")
            },
            new DesignerListener()
            {
                Listen = new Uri("http://255.255.255.255:65535")
            },
            new AddListenerButton()
        };

        public ObservableCollection<RemoteConfigGroup> Remotes => new ObservableCollection<RemoteConfigGroup>()
        {
            new RemoteConfigGroup("Prprpr")
            {
                new RemoteConfig("NameA")
                {
                    Padding = true,
                    Remote = new UriBuilder("https://user:pass@prprpr.pr:3389")
                },
                new RemoteConfig("NameB")
                {
                    Padding = false,
                    Remote = new UriBuilder("https://user:pass@prprpr.pr:3389")
                },
                new RemoteConfig("NameA")
                {
                    Padding = true,
                    Remote = new UriBuilder("https://user:pass@prprpr.pr:3389")
                }
            }
        };
    }

    public class DesignerListener : Listener
    {
        public override Uri Listen { get; set; } = new Uri("socks://127.0.0.1:2333");
        public override bool Enabled => EnabledReal;
        public override bool Running => RunningReal;

        public bool EnabledReal;
        public bool RunningReal;
    }
}
