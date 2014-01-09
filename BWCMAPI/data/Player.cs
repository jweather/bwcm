using System;
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

namespace BWCMAPI.data {
    public class Player {
        // corresponds to a Scala playlist
        public string name;
        public int id;

        public List<Slide> slides = new List<Slide>();

        public Player(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public Player() { }

        static public Predicate<Player> byID(int id) {
            return delegate(Player t) {
                return t.id == id;
            };
        }

        static public Predicate<Player> byName(string name) {
            return delegate(Player t) {
                return t.name == name;
            };
        }

    }
}
