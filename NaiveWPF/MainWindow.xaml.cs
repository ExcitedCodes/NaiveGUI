using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

using fastJSON;
using MaterialDesignThemes.Wpf;
using Hardcodet.Wpf.TaskbarNotification;

using NaiveGUI.View;
using NaiveGUI.Data;
using NaiveGUI.Helper;

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

        public SnackbarMessageQueue snackbarMessageQueue { get; set; } = new SnackbarMessageQueue();

        public string ConfigPath = null;

        public Prop<bool> Logging { get; set; } = new Prop<bool>();
        public Prop<bool> AutoRun { get; set; } = new Prop<bool>();

        public UserControl[] Tabs = null;
        public TabIndexTester CurrentTabTester { get; set; }
        public Prop<int> CurrentTab { get; set; } = new Prop<int>();

        public RemoteConfig CurrentRemote = null;

        public Listener CurrentListener
        {
            get => this._currentListener;
            set
            {
                this._currentListener = value;
                foreach(var l in Listeners)
                {
                    l.Real?.RaisePropertyChanged("Selected");
                }
            }
        }
        private Listener _currentListener = null;

        public SubscriptionTab Subscriptions { get; set; }

        public ObservableCollection<IListener> Listeners { get; set; } = new ObservableCollection<IListener>();
        public ObservableCollection<RemoteConfigGroup> Remotes { get; set; } = new ObservableCollection<RemoteConfigGroup>();

        public MainWindow(string config, bool autorun)
        {
            Instance = this;

            AutoRun.Value = autorun;
            CurrentTabTester = new TabIndexTester(this);
            Subscriptions = new SubscriptionTab(this);

            Tabs = new UserControl[] {
                new ProxyTab(this),
                Subscriptions,
                new LogTab(this),
                new SettingsTab(this),
                new AboutTab()
            };

            InitializeComponent();

            #region Load Config

            if(File.Exists(config))
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
                                group.Add(new RemoteConfig(r.Key, ProxyType.NaiveProxy)
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
                        var listener = new Listener(l["listen"], Enum.Parse(typeof(ProxyType), l["type"]));
                        if(l["remote"] is Dictionary<string, dynamic>)
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
                if(json.ContainsKey("subscriptions"))
                {
                    var sub = (Dictionary<string, dynamic>)json["subscriptions"];
                    foreach(KeyValuePair<string, dynamic> kv in sub["data"])
                    {
                        var s = (Dictionary<string, dynamic>)kv.Value;
                        Subscriptions.Subscriptions.Add(new Subscription(this, kv.Key, s["url"], s["enable"], DateTime.Parse(s["last_update"])));
                    }
                    Subscriptions.LastUpdate.Value = DateTime.Parse(sub["last_update"]);
                    Subscriptions.UpdateInterval = (int)sub["update_interval"];
                }
            }
            if(Remotes.Count == 0)
            {
                Remotes.Add(new RemoteConfigGroup("Default"));
            }
            ConfigPath = config;

            #endregion

            DataContext = this;

            Logging.PropertyChanged += (s, e) => Save();
            Listeners.CollectionChanged += (s, e) => ReloadTrayMenu();

            Listeners.Add(new FakeListener());
            Subscriptions.Subscriptions.Add(new FakeSubscription());

            SwitchTab(0);
        }

        public void Log(string raw) => (Tabs[2] as LogTab).Log(raw);

        public void Save()
        {
            if(ConfigPath == null)
            {
                return;
            }
            File.WriteAllText(ConfigPath, JSON.ToNiceJSON(new Dictionary<string, object>()
            {
                { "version", CONFIG_VERSION },
                { "logging", Logging.Value },
                { "listeners", Listeners.Where(l => l.IsReal).Select(l => new Dictionary<string, object>() {
                    { "type", l.Real.Type.ToString() },
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
                { "subscriptions", new Dictionary<string, object>()
                {
                    { "data",Subscriptions.Subscriptions.Where(s => s.IsReal).ToDictionary(s => s.Real.Name.Value,s => new Dictionary<string,object>()
                    {
                        { "url",s.Real.URL.Value },
                        { "enable",s.Real.Enabled.Value },
                        { "last_update",s.Real.LastUpdate.ToString() }
                    })},
                    { "last_update",Subscriptions.LastUpdate.ToString() },
                    { "update_interval",Subscriptions.UpdateInterval },
                }}
            }));
        }

        public void BalloonTip(string title, string text, int timeout = 3) => trayIcon.ShowBalloonTip(title, text, BalloonIcon.Error);

        #region General Events

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach(var l in Listeners)
            {
                if(l.IsReal && l.Real.Running)
                {
                    l.Real.Stop();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.ReleaseCapture();
            App.SendMessage(new WindowInteropHelper(this).Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        #endregion

        #region Tray Icon & Menu

        private void ReloadTrayMenu()
        {
            // ItemSource Binding will cause severe width issue, so we create menu by code...
            var menu = FindResource("TrayMenu") as ContextMenu;
            while(menu.Items.Count > 2)
            {
                menu.Items.RemoveAt(0);
            }
            for(int i = 0;i < Listeners.Count;i++)
            {
                var item = Listeners[i] as IListener;
                if(item.IsReal)
                {
                    var m = new MenuItem()
                    {
                        Header = item.Real.Listen.ToString()
                    };
                    foreach(var g in Remotes)
                    {
                        var group = new MenuItem()
                        {
                            Header = g.Name
                        };
                        foreach(var r in g)
                        {
                            var remote = new MenuItem()
                            {
                                Header = r.Name,
                                IsCheckable = true,
                                Tag = r
                            };
                            if(item.Real.Remote == r)
                            {
                                remote.IsChecked = group.IsChecked = true;
                            }
                            remote.Click += (se, ev) =>
                            {
                                item.Real.Remote = r;
                                Save();
                            };
                            group.Items.Add(remote);
                        }
                        m.Items.Add(group);
                    }
                    item.Real.PropertyChanged += (se, ev) =>
                    {
                        if(ev.PropertyName != "Remote")
                        {
                            return;
                        }
                        foreach(var g in m.Items)
                        {
                            if(g is Separator)
                            {
                                break;
                            }
                            if(g is MenuItem group)
                            {
                                group.IsChecked = false;
                                foreach(MenuItem remote in group.Items)
                                {
                                    remote.IsChecked = remote.Tag == item.Real.Remote;
                                    if(remote.IsChecked)
                                    {
                                        group.IsChecked = true;
                                    }
                                }
                            }
                        }
                    };
                    m.Items.Add(new Separator());
                    // WPF MenuItem doesn't support SubItem+Clickable, so we add a toggle
                    var toggle = new MenuItem()
                    {
                        Header = "Toggle"
                    };
                    toggle.Click += (se, ev) => item.Real.ToggleEnabled();
                    m.Items.Add(toggle);
                    m.SetBinding(MenuItem.IsCheckedProperty, new Binding("Enabled")
                    {
                        Source = item,
                        Mode = BindingMode.OneWay
                    });
                    menu.Items.Insert(i, m);
                }
            }
        }

        private void TrayIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Show();
            Topmost = true; // Still using this in 2020?
            Topmost = false;
        }

        #endregion

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
