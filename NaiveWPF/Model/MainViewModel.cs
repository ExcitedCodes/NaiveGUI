using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using fastJSON;
using MaterialDesignThemes.Wpf;
using WPFLocalizeExtension.Engine;

using NaiveGUI.Data;
using NaiveGUI.Helper;

namespace NaiveGUI.Model
{
    public class MainViewModel : ModelBase
    {
        public const int CONFIG_VERSION = 2;

        public static LocalizeDictionary Localize => LocalizeDictionary.Instance;

        public static string GetLocalized(string key) => GetLocalized<string>(key);

        public static T GetLocalized<T>(string key) => (T)Localize.GetLocalizedObject(key, null, Localize.Culture);

        public readonly MainWindow View;

        // Tick system, TPS = 5, start from 1
        public ulong Tick = 0;
        public Thread mainTicker = null;

        public string ConfigPath = null;

        public MainViewModel(MainWindow view, string config)
        {
            View = view;

            #region Load Config

            if (File.Exists(config))
            {
                var json = JSON.ToObject<Dictionary<string, dynamic>>(File.ReadAllText(config));
                if (json["version"] == 1)
                {
                    MessageBox.Show("Old winform configuration is not supported, please migrate the config manually or delete config.json", "Opps", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (json["version"] > CONFIG_VERSION && MessageBox.Show(GetLocalized("Message_NewConfigVersion"), "Opps", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }

                SetLanguage(json.ContainsKey("language") ? json["language"] : null, false);

                AllowAddListener = !json.ContainsKey("allow_add_listener") || json["allow_add_listener"];
                AllowWindowResize = !json.ContainsKey("allow_window_resize") || json["allow_window_resize"];
                ScanLeftover = !json.ContainsKey("check_leftover") || json["check_leftover"];

                if (json.ContainsKey("width"))
                {
                    View.Width = json["width"];
                }
                if (json.ContainsKey("height"))
                {
                    View.Height = json["height"];
                }

                if (json.ContainsKey("remotes"))
                {
                    foreach (KeyValuePair<string, object> g in json["remotes"])
                    {
                        try
                        {
                            var group = new RemoteConfigGroup(g.Key, this);
                            foreach (KeyValuePair<string, dynamic> r in (Dictionary<string, dynamic>)g.Value)
                            {
                                try
                                {
                                    group.Add(new RemoteConfig(r.Key)
                                    {
                                        Remote = new UriBuilder(r.Value["remote"]),
                                        ExtraHeaders = r.Value.ContainsKey("extra_headers") ? RemoteConfig.ParseExtraHeaders(r.Value["extra_headers"]) : null
                                    });
                                }
                                catch (Exception e)
                                {
                                    if (MessageBox.Show(string.Format(GetLocalized("Message_RemoteParseError"), g.Key, e.ToString()), "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                            }
                            Remotes.Add(group);
                        }
                        catch (Exception e)
                        {
                            if (MessageBox.Show(string.Format(GetLocalized("Message_GroupParseError"), g.Key, e.ToString()), "Error", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }

                if (json.ContainsKey("listeners"))
                {
                    foreach (var l in json["listeners"])
                    {
                        var listener = new Listener(l["listen"]);
                        if (l["remote"] is Dictionary<string, dynamic>)
                        {
                            var name = l["remote"]["name"];
                            var group = l["remote"]["group"];
                            foreach (var g in Remotes)
                            {
                                if (g.Name == group)
                                {
                                    foreach (var r in g)
                                    {
                                        if (r.Name == name)
                                        {
                                            listener.Remote = r;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        if (l.ContainsKey("enable") && l["enable"])
                        {
                            listener.Enabled = true;
                        }
                        Listeners.Add(listener);
                    }
                }

                if (json.ContainsKey("subscriptions"))
                {
                    var sub = (Dictionary<string, dynamic>)json["subscriptions"];
                    foreach (KeyValuePair<string, dynamic> kv in sub["data"])
                    {
                        var s = (Dictionary<string, dynamic>)kv.Value;
                        Subscriptions.Add(new Subscription(this, kv.Key, s["url"], s["enable"], DateTime.Parse(s["last_update"])));
                    }
                    SubscriptionLastUpdate = DateTime.Parse(sub["last_update"]);
                    SubscriptionUpdateInterval = (int)sub["update_interval"];
                }
            }

            // TODO: This will conflict with listeners loading logic
            if (ScanLeftover)
            {
                SearchLeftoverProcesses();
            }

            if (Remotes.Count == 0)
            {
                Remotes.Add(new RemoteConfigGroup("Default", this));
            }
            ConfigPath = config;

            #endregion

            Remotes.CollectionChanged += (s, e) => ReloadTrayMenu();
            Listeners.CollectionChanged += (s, e) => ReloadTrayMenu();

            Listeners.Add(new FakeListener(this));
            Subscriptions.Add(new FakeSubscription());

            SwitchTab(0);

            ReloadTrayMenu();

            mainTicker = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Tick++;
                    try
                    {
                        foreach (var l in Listeners)
                        {
                            if (l.IsReal)
                            {
                                l.Real.Tick();
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch { }
                    Thread.Sleep(200);
                }
            }))
            {
                IsBackground = true
            };
            mainTicker.Start();

            subscriptionTimer = new Timer(s => UpdateSubscription(), null, 0, 60000);

            Microsoft.Win32.SystemEvents.SessionEnding += (s, e) =>
            {
                ConfigPath = null;
                mainTicker.Abort();

                foreach (var l in Listeners)
                {
                    if (l.IsReal)
                    {
                        l.Real.Enabled = false;
                        l.Real.Stop();
                    }
                }
            };
        }

        public SnackbarMessageQueue snackbarMessageQueue { get; set; } = new SnackbarMessageQueue();

        public string SelectedLanguage = null;

        public RemoteConfig CurrentRemote = null;

        public Listener CurrentListener
        {
            get => _currentListener;
            set
            {
                Set(out _currentListener, value);
                foreach (var l in Listeners)
                {
                    if (!l.IsReal)
                    {
                        continue;
                    }
                    l.Real.Selected = l.Real == _currentListener;
                }
            }
        }
        private Listener _currentListener = null;

        #region NaiveGUI Basics

        public bool AutoRun
        {
            get => File.Exists(App.AutoRunFile);
            set
            {
                App.SetAutoRun(!AutoRun);
                RaisePropertyChanged();
            }
        }

        public bool AllowAddListener { get => _allowAddListener; set => Set(out _allowAddListener, value); }
        private bool _allowAddListener = true;

        public bool AllowWindowResize
        {
            get => _allowWindowResize;
            set
            {
                Set(out _allowWindowResize, value);
                View.ResizeMode = value ? ResizeMode.CanResize : ResizeMode.CanMinimize;
            }
        }
        private bool _allowWindowResize = true;

        public bool ScanLeftover { get => _scanLeftover; set => Set(out _scanLeftover, value); }
        private bool _scanLeftover = true;

        public ObservableCollection<IListener> Listeners { get; set; } = new ObservableCollection<IListener>();
        public ObservableCollection<RemoteConfigGroup> Remotes { get; set; } = new ObservableCollection<RemoteConfigGroup>();

        #endregion

        #region Tabs

        public int CurrentTab { get => _currentTab; set => Set(out _currentTab, value); }
        private int _currentTab = -1;

        [SourceBinding(nameof(CurrentTab))]
        public TabIndexTester CurrentTabTester { get; set; }

        public void SwitchTab(int id)
        {
            if (CurrentTab != id)
            {
                CurrentTab = id;
                View.BeginTabStoryboard("TabHideAnimation");
            }
        }

        #endregion

        #region Subscription

        public bool SubscriptionAutoUpdate
        {
            get => SubscriptionUpdateInterval != -1;
            set
            {
                SubscriptionUpdateInterval = value ? 7200 : -1;
                RaisePropertyChanged();
            }
        }

        public int SubscriptionUpdateInterval
        {
            get => _subscriptionUpdateInterval;
            set
            {
                if (_subscriptionUpdateInterval != value)
                {
                    _subscriptionUpdateInterval = value;
                    RaisePropertyChanged();
                    Save();
                }
            }
        }
        private int _subscriptionUpdateInterval = -1;

        public bool SubscriptionUpdating { get => _subscriptionUpdating; set => Set(out _subscriptionUpdating, value); }
        public bool _subscriptionUpdating = false;

        public Timer subscriptionTimer = null;
        public DateTime SubscriptionLastUpdate = DateTime.MinValue;

        public ObservableCollection<ISubscription> Subscriptions { get; private set; } = new ObservableCollection<ISubscription>();

        public int UpdateSubscription(bool silent = true, bool force = false)
        {
            if (!force && (SubscriptionUpdateInterval < 0 || (DateTime.Now - SubscriptionLastUpdate).TotalSeconds < SubscriptionUpdateInterval))
            {
                return -1;
            }
            int count = 0;
            foreach (var s in Subscriptions)
            {
                if (s.IsReal && s.Real.Enabled && s.Real.Update(silent))
                {
                    count++;
                }
            }
            SubscriptionLastUpdate = DateTime.Now;
            Save();
            return count;
        }

        #endregion

        public void ReloadTrayMenu()
        {
            // ItemSource Binding will cause severe width issue, create menu manually.
            var menu = View.FindResource("TrayMenu") as ContextMenu;
            while (menu.Items.Count > 2)
            {
                menu.Items.RemoveAt(0);
            }
            if (Listeners.Count == 1 && Listeners[0] is FakeListener)
            {
                var item = new MenuItem()
                {
                    Header = GetLocalized("Tray_NoListener"),
                    IsEnabled = false
                };
                item.Items.Add(new MenuItem()); // Placeholder
                menu.Items.Insert(0, item);
                return;
            }
            for (int i = 0; i < Listeners.Count; i++)
            {
                var item = Listeners[i];
                if (item.IsReal)
                {
                    var m = new MenuItem()
                    {
                        Header = item.Real.Listen.ToString()
                    };

                    if (Remotes.Count == 0 || (Remotes.Count == 1 && Remotes[0].Count == 0))
                    {
                        m.Items.Add(new MenuItem()
                        {
                            Header = GetLocalized("Tray_NoRemote"),
                            IsEnabled = false
                        });
                    }
                    else if (Remotes.Count > 1)
                    {
                        foreach (var g in Remotes)
                        {
                            var group = new MenuItem()
                            {
                                Header = g.Name,
                                Tag = g
                            };
                            foreach (var r in g)
                            {
                                var remote = new MenuItem()
                                {
                                    Header = r.Name,
                                    IsCheckable = true,
                                    Tag = r
                                };
                                if (item.Real.Remote == r)
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
                            if (ev.PropertyName != "Remote")
                            {
                                return;
                            }
                            foreach (var g in m.Items)
                            {
                                if (g is Separator)
                                {
                                    break;
                                }
                                if (g is MenuItem group)
                                {
                                    group.IsChecked = false;
                                    foreach (MenuItem remote in group.Items)
                                    {
                                        remote.IsChecked = remote.Tag == item.Real.Remote;
                                        if (remote.IsChecked)
                                        {
                                            group.IsChecked = true;
                                        }
                                    }
                                }
                            }
                        };
                    }
                    else
                    {
                        foreach (var r in Remotes[0])
                        {
                            var remote = new MenuItem()
                            {
                                Header = r.Name,
                                IsCheckable = true,
                                Tag = r,
                                IsChecked = item.Real.Remote == r
                            };
                            remote.Click += (se, ev) =>
                            {
                                item.Real.Remote = r;
                                Save();
                            };
                            m.Items.Add(remote);
                        }
                        item.Real.PropertyChanged += (se, ev) =>
                        {
                            if (ev.PropertyName != "Remote")
                            {
                                return;
                            }
                            foreach (var g in m.Items)
                            {
                                if (g is Separator)
                                {
                                    break;
                                }
                                if (g is MenuItem tmp)
                                {
                                    tmp.IsChecked = tmp.Tag == item.Real.Remote;
                                }
                            }
                        };
                    }

                    m.Items.Add(new Separator());

                    // WPF MenuItem doesn't support SubItem+Clickable, so we add a toggle
                    var toggle = new MenuItem()
                    {
                        Header = GetLocalized("Tray_Toggle")
                    };
                    toggle.Click += (se, ev) => item.Real.Enabled = !item.Real.Enabled;
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

        public void Save()
        {
            if (ConfigPath == null)
            {
                return;
            }
            File.WriteAllText(ConfigPath, JSON.ToNiceJSON(new Dictionary<string, object>()
            {
                { "version", CONFIG_VERSION },
                { "width", View.Dispatcher.Invoke(() => (int)View.Width) },
                { "height", View.Dispatcher.Invoke(() => (int)View.Height) },
                { "allow_add_listener", AllowAddListener },
                { "allow_window_resize", AllowWindowResize },
                { "check_leftover", ScanLeftover },
                { "language", SelectedLanguage },
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
                    { "extra_headers", r.ExtraHeaders ?? new string[0] }
                })) },
                { "subscriptions", new Dictionary<string, object>() {
                    { "data", Subscriptions.Where(s => s.IsReal).ToDictionary(s => s.Real.Name,s => new Dictionary<string,object>() {
                        { "url",s.Real.URL },
                        { "enable",s.Real.Enabled },
                        { "last_update",s.Real.LastUpdate.ToString() }
                    })},
                    { "last_update",SubscriptionLastUpdate.ToString() },
                    { "update_interval",SubscriptionUpdateInterval },
                }}
            }));
        }

        public void SetLanguage(string lang, bool save = true)
        {
            Localize.Culture = lang == null ? CultureInfo.CurrentCulture : CultureInfo.CreateSpecificCulture(lang);
            SelectedLanguage = lang;
            if (save)
            {
                Save();
            }
        }

        public void SearchLeftoverProcesses()
        {
            if (!ScanLeftover)
            {
                return;
            }
            var test = Path.GetFullPath(Listener.NaivePath);
            var processes = Process.GetProcessesByName("naive").Where(p =>
            {
                try
                {
                    uint bufferSize = 256;
                    var sb = new StringBuilder((int)bufferSize - 1);
                    if (App.QueryFullProcessImageName(p.Handle, 0, sb, ref bufferSize))
                    {
                        return Path.GetFullPath(sb.ToString()) == test;
                    }
                }
                catch { }
                return false;
            }).ToArray();
            if (processes.Length != 0)
            {
                switch (MessageBox.Show(string.Format(GetLocalized("Message_LeftoverFound"), processes.Length), "Oops", MessageBoxButton.YesNoCancel, MessageBoxImage.Information))
                {
                case MessageBoxResult.Yes:
                    foreach (var p in processes)
                    {
                        try
                        {
                            p.Kill();
                            p.WaitForExit(100); // TODO: Customizable wait timeout
                        }
                        catch { }
                    }
                    break;
                case MessageBoxResult.No:
                    break;
                default:
                    Environment.Exit(0);
                    break;
                }
            }
        }
    }
}
