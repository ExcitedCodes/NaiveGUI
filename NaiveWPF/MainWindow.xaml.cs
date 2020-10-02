using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;

using fastJSON;
using MaterialDesignThemes.Wpf;
using WPFLocalizeExtension.Engine;
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

        public static MainWindow Instance = null;

        public static LocalizeDictionary Localize => LocalizeDictionary.Instance;

        public static string GetLocalized(string key) => GetLocalized<string>(key);

        public static T GetLocalized<T>(string key) => (T)Localize.GetLocalizedObject(key, null, Localize.Culture);

        public SnackbarMessageQueue snackbarMessageQueue { get; set; } = new SnackbarMessageQueue();

        public string ConfigPath = null;

        public string SelectedLanguage = null;

        public Prop<bool> Logging { get; set; } = new Prop<bool>();
        public Prop<bool> AutoRun { get; set; } = new Prop<bool>();
        public Prop<bool> AllowAddListener { get; set; } = new Prop<bool>(true);
        public Prop<bool> AllowWindowResize { get; set; } = new Prop<bool>(true);
        public Prop<bool> CheckLeftover { get; set; } = new Prop<bool>(true);

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

        // Tick system, TPS = 5, start from 1
        public ulong Tick = 0;
        public Timer mainTimer = null;

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
                var json = JSON.ToObject<Dictionary<string, dynamic>>(File.ReadAllText(config));
                if(json["version"] == 1)
                {
                    MessageBox.Show("Old winform configuration is not supported, please migrate the config manually or delete config.json", "Opps", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                if (json["version"] > CONFIG_VERSION && MessageBox.Show(GetLocalized("Message_NewConfigVersion"), "Opps", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }
                SetLanguage(json.ContainsKey("language") ? json["language"] : null, false);
                Logging.Value = json.ContainsKey("logging") && json["logging"];
                AllowAddListener.Value = !json.ContainsKey("allow_add_listener") || json["allow_add_listener"];
                AllowWindowResize.Value = !json.ContainsKey("allow_window_resize") || json["allow_window_resize"];
                CheckLeftover.Value = !json.ContainsKey("check_leftover") || json["check_leftover"];
                if (json.ContainsKey("width"))
                {
                    Width = json["width"];
                }
                if(json.ContainsKey("height"))
                {
                    Height = json["height"];
                }
                if(json.ContainsKey("remotes"))
                {
                    foreach(KeyValuePair<string, object> g in json["remotes"])
                    {
                        try
                        {
                            var group = new RemoteConfigGroup(g.Key, this);
                            foreach (KeyValuePair<string, dynamic> r in (Dictionary<string, dynamic>)g.Value)
                            {
                                try
                                {
                                    group.Add(new RemoteConfig(r.Key, ProxyType.NaiveProxy)
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
                
                SearchLeftoverProcesses(); // TODO: Get this out of config loading procedue

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
            else
            {
                SearchLeftoverProcesses(); // TODO: Again, get this out of config loading procedue
            }

            if (Remotes.Count == 0)
            {
                Remotes.Add(new RemoteConfigGroup("Default", this));
            }
            ConfigPath = config;
            ResizeMode = AllowWindowResize.Value ? ResizeMode.CanResize : ResizeMode.CanMinimize;

            #endregion

            DataContext = this;

            Logging.PropertyChanged += (s, e) => Save();
            AllowAddListener.PropertyChanged += (s, e) => Save();
            CheckLeftover.PropertyChanged += (s, e) => Save();
            AllowWindowResize.PropertyChanged += (s, e) =>
            {
                ResizeMode = AllowWindowResize.Value ? ResizeMode.CanResize : ResizeMode.CanMinimize;
                Save();
            };
            Remotes.CollectionChanged += (s, e) => ReloadTrayMenu();
            Listeners.CollectionChanged += (s, e) => ReloadTrayMenu();

            Listeners.Add(new FakeListener());
            Subscriptions.Subscriptions.Add(new FakeSubscription());

            SwitchTab(0);

            ReloadTrayMenu();

            mainTimer = new Timer((s) =>
            {
                Tick++;
                foreach(var l in Listeners)
                {
                    if(l.IsReal)
                    {
                        l.Real.Tick(Tick);
                    }
                }
            }, null, 0, 200);

            Microsoft.Win32.SystemEvents.SessionEnding += (s, e) =>
            {
                ConfigPath = null;
                mainTimer.Change(Timeout.Infinite, Timeout.Infinite);

                foreach(var l in Listeners)
                {
                    if(l.IsReal)
                    {
                        l.Real.Stop();
                    }
                }
            };
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
                { "width", Dispatcher.Invoke(() => (int)Width) },
                { "height", Dispatcher.Invoke(() => (int)Height) },
                { "allow_add_listener", AllowAddListener.Value },
                { "allow_window_resize", AllowWindowResize.Value },
                { "check_leftover", CheckLeftover.Value },
                { "language", SelectedLanguage },
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
                    { "extra_headers", r.ExtraHeaders ?? new string[0] }
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
            if(CheckLeftover.Value)
            {
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) => Save();

        private void ButtonHide_Click(object sender, RoutedEventArgs e) => Hide();

        #endregion

        #region Tray Icon & Menu

        public void ReloadTrayMenu()
        {
            // ItemSource Binding will cause severe width issue, so we create menu by code...
            var menu = FindResource("TrayMenu") as ContextMenu;
            while(menu.Items.Count > 2)
            {
                menu.Items.RemoveAt(0);
            }
            if(Listeners.Count == 1 && Listeners[0] is FakeListener)
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
            for(int i = 0;i < Listeners.Count;i++)
            {
                var item = Listeners[i] as IListener;
                if(item.IsReal)
                {
                    var m = new MenuItem()
                    {
                        Header = item.Real.Listen.ToString()
                    };
                    if(Remotes.Count == 0 || (Remotes.Count == 1 && Remotes[0].Count == 0))
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
