using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace BWCMAPI.data {
    public class BUser {
        public static List<BUser> users = new List<BUser>();
        public static Dictionary<string, BUser> byName = new Dictionary<string, BUser>();

        public static void clear() {
            users.Clear(); byName.Clear();
        }

        public static BUser find(string name) {
            if (!byName.ContainsKey(name)) return null;
            return byName[name];
        }

        public string user;

        public void save() {
            if (byName.ContainsKey(user)) {
                BUser old = byName[user];
                byName.Remove(user);
                users.Remove(old);
            }

            byName.Add(user, this);
            users.Add(this);

            if (password != null) {
                setPassword(password);
                password = null;
            }
        }
        public string passhash, password;
        public bool isAdmin;
        public List<string> players = new List<string>();

        public BUser() { }

        public void delete() {
            users.Remove(this);
            byName.Remove(this.user);
        }

        // todo password hashing
        public void setPassword(string pass) {
            passhash = pass;
        }

        public bool checkPassword(string test) {
            return (passhash == test);
        }
    }
}
