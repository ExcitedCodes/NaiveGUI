using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

using fastJSON;

using NaiveGUI.Data;

namespace NaiveGUI.Model
{
    /// <summary>
    /// 订阅的数据对象, 每个订阅 URL 可能包含多个服务器组
    /// </summary>
    public class SubscriptionModel : ModelBase, ISubscription
    {
        private readonly MainViewModel Main;

        public string Name { get => _name; set => Set(out _name, value); }
        public string _name = "";

        public string URL { get => _url; set => Set(out _url, value); }
        public string _url = "";

        public bool Enabled { get => _enabled; set => Set(out _enabled, value); }
        public bool _enabled = false;

        public DateTime LastUpdate { get => _lastUpdate; set => Set(out _lastUpdate, value); }
        public DateTime _lastUpdate = DateTime.MinValue;

        public bool IsReal => true;
        public SubscriptionModel Real => this;

        public SubscriptionModel(MainViewModel main, string name, string url, bool enabled, DateTime lastUpdate)
        {
            Main = main;
            Name = name;
            URL = url;
            Enabled = enabled;
            LastUpdate = lastUpdate;
        }

        /// <summary>
        /// 进行订阅更新.
        /// 更新时：
        /// 1. 由组名-名称组成键进行匹配
        /// 2. 匹配到的服务器更新
        /// 3. 未匹配到的服务器进行添加
        /// 4. 扫描整个组, 订阅 URL 中不存在的键将被删除
        /// </summary>
        /// <param name="silent">不弹出错误提示</param>
        /// <returns>是否更新成功</returns>
        public bool Update(bool silent)
        {
            /*
            {
                "GroupName1": [
                    {
                        "name": "Name here!",
                        "host": "xxx.xxx",
                        "port": 2333,
                        "scheme": "https", // optional
                        "username": "UserXD", // optional
                        "password": "Password0", // optional
                        "extra_headers": [ // optional, must be a string array
                            "HeaderAAAAA: WTFWTF",
                            "YAAY: LOLL",
                            ...
                        ]
                    },
                    ...
                ],
                "GroupName2": [
                    ...
                ]
            }
            */
            try
            {
                Dictionary<string, List<dynamic>> json;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

                var request = WebRequest.CreateHttp(URL);
                request.Method = "GET";
                request.Timeout = 10000;
                request.UserAgent = "NaiveGUI/" + Assembly.GetExecutingAssembly().GetName().Version;
                request.AllowAutoRedirect = true;

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Bad HTTP Status: " + response.StatusCode + " " + response.StatusDescription);
                    }
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        json = JSON.ToObject<Dictionary<string, List<dynamic>>>(sr.ReadToEnd());
                    }
                }

                Main.View.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        var mainRemotes = Main.Remotes;
                        var subscribeGroups = new Dictionary<string, List<string>>();

                        // Add or update remotes
                        foreach (var kv in json)
                        {
                            var group = kv.Key;
                            subscribeGroups.Add(group, new List<string>());
                            foreach (Dictionary<string, dynamic> r in kv.Value)
                            {
                                // Can't find a better solution in current
                                string name = r["name"], host = r["host"].ToString(), scheme = r.ContainsKey("scheme") ? r["scheme"] : "https",
                                    username = r.ContainsKey("username") ? r["username"].ToString() : null, password = r.ContainsKey("password") ? r["password"].ToString() : null;
                                int port = r.ContainsKey("port") ? (r["port"] is string ? int.Parse(r["port"]) : (int)r["port"]) : -1;
                                string[] extra_headers = r.ContainsKey("extra_headers") ? RemoteModel.ParseExtraHeaders(r["extra_headers"]) : null;
                                subscribeGroups[group].Add(name);
                                foreach (var g in mainRemotes)
                                {
                                    if (g.Name == group)
                                    {
                                        foreach (var rMain in g)
                                        {
                                            if (rMain.Name == name)
                                            {
                                                rMain.Remote.Host = host;
                                                rMain.Remote.Port = port;
                                                rMain.Remote.Scheme = scheme;
                                                rMain.Remote.UserName = username;
                                                rMain.Remote.Password = password;
                                                rMain.ExtraHeaders = extra_headers;
                                                goto CONTINUE2;
                                            }
                                        }
                                        g.Add(new RemoteModel(name)
                                        {
                                            Remote = new UriBuilder(scheme, host, port)
                                            {
                                                UserName = username,
                                                Password = password
                                            },
                                            ExtraHeaders = extra_headers
                                        });
                                        goto CONTINUE2;
                                    }
                                }
                                mainRemotes.Add(new RemoteGroupModel(group, Main)
                                {
                                    new RemoteModel(name)
                                    {
                                        Remote = new UriBuilder(scheme, host, port)
                                        {
                                            UserName = username,
                                            Password = password
                                        },
                                        ExtraHeaders = extra_headers
                                    }
                                });
                            CONTINUE2:;
                            }
                        }

                        // Remove remotes
                        foreach (var kv in subscribeGroups)
                        {
                            foreach (var g in mainRemotes)
                            {
                                if (g.Name == kv.Key)
                                {
                                    for (int i = 0; i < g.Count; i++)
                                    {
                                        if (!kv.Value.Contains(g[i].Name))
                                        {
                                            g.RemoveAt(i--);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        LastUpdate = DateTime.Now;
                        Main.Save();
                        Main.Log("Subscription updated: " + Name);
                    }
                    catch (Exception e)
                    {
                        if (!silent)
                        {
                            MessageBox.Show("Unable to update " + Name + ": " + e.Message + ".\n\n" + e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        Main.Log("Subscription " + Name + ": " + e.ToString(), 3);
                    }
                });
                return true;
            }
            catch (Exception e)
            {
                if (!silent)
                {
                    MessageBox.Show("Unable to update " + Name + ": " + e.Message + ".\n\n" + e.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Main.Log("Subscription " + Name + ": " + e.ToString(), 3);
            }
            return false;
        }
    }
}
