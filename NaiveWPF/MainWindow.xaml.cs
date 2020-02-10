using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

using fastJSON;
using Hardcodet.Wpf.TaskbarNotification;

using NaiveGUI.View;
using NaiveGUI.Data;

namespace NaiveGUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public const int CONFIG_VERSION = 2;

        public static ulong Tick = 0;
        public static MainWindow Instance = null;

        public string ConfigPath = null;
        
        public Prop<bool> Logging { get; set; } = new Prop<bool>();
        public Prop<bool> AutoRun { get; set; } = new Prop<bool>();

        public UserControl[] Tabs = null;
        public Prop<int> CurrentTab { get; set; } = new Prop<int>();

        public Listener CurrentListener
        {
            get => this._currentListener;
            set
            {
                this._currentListener = value;
                foreach(var l in Listeners)
                {
                    if(l.IsReal)
                    {
                        l.Real.RaisePropertyChanged("ShadowOpacity");
                    }
                }
            }
        }
        private Listener _currentListener = null;

        public RemoteConfig CurrentRemote = null;

        public ObservableCollection<IListener> Listeners { get; set; } = new ObservableCollection<IListener>();
        public ObservableCollection<RemoteConfigGroup> Remotes { get; set; } = new ObservableCollection<RemoteConfigGroup>();

        public TabIndexTester CurrentTabTester { get; set; }

        public MainWindow(string config, bool autorun)
        {
            Instance = this;

            Tabs = new UserControl[] {
                new ProxyTab(this),
                new SubscriptionTab(this),
                new LogTab(this),
                new SettingsTab(this)
            };
            AutoRun.Value = autorun;
            CurrentTabTester = new TabIndexTester(this);

            InitializeComponent();

            #region Load Config

            ConfigPath = config;
            if(File.Exists(ConfigPath))
            {
                var json = JSON.ToObject<Dictionary<string, dynamic>>(File.ReadAllText("config.json"));
                if(json["version"] == 1)
                {
                    // TODO: Migrate
                    throw new NotImplementedException();
                }
                if(json["version"] > CONFIG_VERSION && MessageBox.Show("The config.json has a newer version, continue loading may lost some config. Continue?", "Opps", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }
                Logging.Value = json.ContainsKey("logging") && json["logging"];
                if(json.ContainsKey("remotes"))
                {
                    foreach(KeyValuePair<string, object> g in json["remotes"])
                    {
                        try
                        {
                            var group = new RemoteConfigGroup(g.Key);
                            foreach(KeyValuePair<string, dynamic> r in (Dictionary<string, dynamic>)g.Value)
                            {
                                group.Add(new RemoteConfig(r.Key)
                                {
                                    Remote = new UriBuilder(r.Value["remote"]),
                                    Padding = r.Value.ContainsKey("padding") && r.Value["padding"],
                                });
                            }
                            Remotes.Add(group);
                        }
                        catch { }
                    }
                }
                if(json.ContainsKey("listeners"))
                {
                    foreach(var l in json["listeners"])
                    {
                        var listener = new Listener(l["listen"]);
                        if(l["remote"] is Dictionary<string,dynamic>)
                        {
                            var name = l["remote"]["name"];
                            var group = l["remote"]["group"];
                            foreach(var g in Remotes)
                            {
                                if(g.Name == group)
                                {
                                    foreach(var r in g)
                                    {
                                        if(r.Name == name)
                                        {
                                            listener.Remote = r;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        if(l.ContainsKey("enable") && l["enable"])
                        {
                            listener.ToggleEnabled();
                        }
                        Listeners.Add(listener);
                    }
                }
                /*
                if(json.ContainsKey("subscriptions"))
                {
                    var sub = (Dictionary<string, dynamic>)json["subscriptions"];
                    Subscriptions.LastUpdate = DateTime.Parse(sub["last_update"]);
                    Subscriptions.UpdateInterval = (int)sub["update_interval"];
                    foreach(Dictionary<string, dynamic> s in sub["data"])
                    {
                        Subscriptions.Add(new SubscriptionData()
                        {
                            URL = s["url"],
                            LastUpdate = DateTime.Parse(s["last_update"])
                        });
                    }
                }
                */
            }

            #endregion

            DataContext = this;

            Logging.PropertyChanged += (s, e) => Save();

            Listeners.Add(new AddListenerButton());

            SwitchTab(0);
        }

        public void Save()
        {
            (Tabs[0] as ProxyTab).SayWTF();
            File.WriteAllText(ConfigPath, JSON.ToNiceJSON(new Dictionary<string, object>()
            {
                { "version", CONFIG_VERSION },
                { "logging", Logging.Value },
                { "listeners", Listeners.Where(l => l.IsReal).Select(l => new Dictionary<string, object>() {
                    { "enable", l.Real.Enabled },
                    { "listen", l.Real.Listen.ToString() },
                    { "remote", l.Real.Remote == null ? null : new Dictionary<string,object>() {
                        { "name", l.Real.Remote.Name },
                        { "group",l.Real.Remote.Group.Name }
                    }}
                }) },
                { "remotes", Remotes.ToDictionary(g => g.Name,g => g.ToDictionary(r => r.Name,r => new Dictionary<string,object>() {
                    { "remote", r.Remote.ToString() },
                    { "padding", r.Padding }
                })) },
                /*
                { "subscriptions", new Dictionary<string, object>()
                {
                    { "data",Subscriptions.Select(s => new Dictionary<string,object>()
                    {
                        { "url",s.URL },
                        { "last_update",s.LastUpdate.ToString() }
                    })},
                    { "last_update",Subscriptions.LastUpdate.ToString() },
                    { "update_interval",Subscriptions.UpdateInterval },
                }}
                */
            }));
        }

        public void Log(string raw) => (Tabs[2] as LogTab).Log(raw);

        private void TrayIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e) => Show();

        public void BalloonTip(string title, string text, int timeout = 3) => trayIcon.ShowBalloonTip(title, text, BalloonIcon.Error);

        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            App.ReleaseCapture();
            App.SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        #region Tab Switching

        public void SwitchTab(int id)
        {
            CurrentTab.Value = id;
            tabContents.BeginStoryboard(Resources["TabHideAnimation"] as Storyboard);
        }

        private void ButtonTab_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((sender as Button).Tag as string);
            if(CurrentTab.Value != id)
            {
                SwitchTab(id);
                RaisePropertyChanged("CurrentTabTester");
            }
        }

        private void StoryboardTabHideAnimation_Completed(object sender, EventArgs e)
        {
            tabContents.Child = Tabs[CurrentTab.Value];
            tabContents.BeginStoryboard(Resources["TabShowAnimation"] as Storyboard);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
