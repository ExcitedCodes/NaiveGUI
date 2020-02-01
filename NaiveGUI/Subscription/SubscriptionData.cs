using System;
using System.Windows.Forms;
using System.Collections.Generic;

using fastJSON;

namespace NaiveGUI.Subscription
{
    /// <summary>
    /// 订阅的数据对象, 每个订阅 URL 可能包含多个服务器组
    /// </summary>
    public class SubscriptionData
    {
        /// <summary>
        /// 更新时需拉取数据的地址
        /// </summary>
        public string URL = null;

        public DateTime LastUpdate = DateTime.MinValue;

        public string GetLastUpdate() => LastUpdate == DateTime.MinValue ? "-" : LastUpdate.ToLongTimeString();

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
                        "scheme": "socks", // optional
                        "username": "UserXD", // optional
                        "password": "Password0", // optional
                        "padding": true, // optional
                        "quic_version": -1 // optional
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
                var json = JSON.ToObject<Dictionary<string, List<dynamic>>>(Utils.HttpGetString(URL));
                var mainRemotes = MainForm.Instance.Remotes;
                var subscribeGroups = new Dictionary<string, List<string>>();
                // Add or update remotes
                foreach(var kv in json)
                {
                    var group = kv.Key;
                    subscribeGroups.Add(group, new List<string>());
                    foreach(Dictionary<string, dynamic> r in kv.Value)
                    {
                        // Can't find a better solution in current
                        string name = r["name"], host = r["host"], scheme = r.ContainsKey("scheme") ? r["scheme"] : "socks",
                            username = r.ContainsKey("username") ? r["username"] : null, password = r.ContainsKey("password") ? r["password"] : null;
                        int port = r.ContainsKey("port") ? (r["port"] is string ? int.Parse(r["port"]) : (int)r["port"]) : -1,
                            quic_version = r.ContainsKey("quic_version") ? (r["quic_version"] is string ? int.Parse(r["quic_version"]) : (int)r["quic_version"]) : -1;
                        bool padding = r.ContainsKey("padding") ? (r["padding"] is string ? bool.Parse(r["padding"]) : r["padding"]) : false;
                        subscribeGroups[group].Add(name);
                        foreach(var rMain in mainRemotes)
                        {
                            if(rMain.Group == group && rMain.Name == name)
                            {
                                rMain.Remote.Host = host;
                                rMain.Remote.Port = port;
                                rMain.Remote.Scheme = scheme;
                                rMain.Remote.UserName = username;
                                rMain.Remote.Password = password;
                                rMain.Padding = padding;
                                rMain.QuicVersion = quic_version;
                                goto CONTINUE2;
                            }
                        }
                        mainRemotes.Add(new RemoteConfig(group, name)
                        {
                            Remote = new UriBuilder(scheme, host, port)
                            {
                                UserName = username,
                                Password = password
                            },
                            Padding = padding,
                            QuicVersion = quic_version
                        });
                    CONTINUE2:;
                    }
                }
                // Remove remotes
                foreach(var kv in subscribeGroups)
                {
                    for(int i = 0;i < mainRemotes.Count;i++)
                    {
                        if(mainRemotes[i].Group == kv.Key && !kv.Value.Contains(mainRemotes[i].Name))
                        {
                            mainRemotes.RemoveAt(i--);
                        }
                    }
                }
                LastUpdate = DateTime.Now;
                MainForm.Instance.Save();
                MainForm.Instance.RefreshRemoteTree(false);
                if(MainForm.Instance.CurrentRemote != null)
                {
                    MainForm.Instance.LoadConfigUI(MainForm.Instance.CurrentRemote);
                }
                return true;
            }
            catch(Exception e)
            {
                if(!silent)
                {
                    MessageBox.Show("Unable to update subscription '" + URL + "': " + e.Message + ".\n\n" + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }
    }
}
