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
    public class Slide {
        // corresponds to a Scala message
        public int id;
        public string name;
        public int templateID;
        public bool changed = false;
        public List<Field> fields = new List<Field>();

        public Slide(int id, string name) {
            this.id = id; this.name = name;
        }

        public Slide() { this.id = -1;  }

        static public Predicate<Slide> byID(int id) {
            return delegate(Slide t) {
                return t.id == id;
            };
        }
    }
}
