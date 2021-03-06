﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace BWCMAPI.data {
    public class Persistence {
        private static Object sync = new Object();
        public static void loadData() {
            // clear old data
            BUser.clear();

            string path = HostingEnvironment.MapPath("~/App_Data/data.json");
            FileStream fs = File.OpenRead(path);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            string str = Encoding.ASCII.GetString(buffer);
            Dictionary<string, Object> data = (Dictionary<string, Object>) Global.json.DeserializeObject(str);

            if (data.ContainsKey("users")) {
                Object[] users = (Object[])data["users"];
                foreach (Object ou in users) {
                    BUser u = (BUser)ou;
                    u.save();
                }
            }

            if (data.ContainsKey("widgets")) {
                Object[] list = (Object[])data["widgets"];
                Widget.renderList = new List<Widget>();
                foreach (Object o in list) {
                    Widget w = (Widget)o;
                    Widget.renderList.Add(w);
                }
            }

            if (data.ContainsKey("players")) {
                Dictionary<string, Object> pinfo = (Dictionary<string, Object>)data["players"];
                Player.playerInfo = new Dictionary<string, PlayerInfo>();
                foreach (string key in pinfo.Keys) {
                    Player.playerInfo[key] = (PlayerInfo)pinfo[key];
                }
            }

            Global.d("loaded " + BUser.users.Count + " users and " + Widget.renderList.Count + " widgets from data.json");
        }

        public static void saveData() {
            FileStream fs = null;
            try {
                lock (sync) {
                    if (!Global.dataDirty) return;

                    Dictionary<string, Object> data = new Dictionary<string, object>();
                    data.Add("users", BUser.users);
                    data.Add("widgets", Widget.renderList);
                    Dictionary<string, PlayerInfo> pinfo = new Dictionary<string, PlayerInfo>();
                    foreach (Player p in Scala.players)
                        pinfo.Add(p.name, p.info);
                    data.Add("players", pinfo);

                    string path = HostingEnvironment.MapPath("~/App_Data/data.json");
                    string str = Global.json.Serialize(data);
                    fs = File.Open(path, FileMode.Truncate);
                    byte[] buffer = Encoding.ASCII.GetBytes(str);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                    Global.dataDirty = false;
                    Global.d("wrote " + BUser.users.Count + " users and " + Widget.renderList.Count + " widgets to data.json");
                }
            } catch (Exception e) {
                Global.error("Failed to persist data.json: " + e);
            } finally {
                if (fs != null) try { fs.Close(); } catch { }
            }
        }
    }
}
