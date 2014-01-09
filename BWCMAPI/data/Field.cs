using System;
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
    public class Field {
        public string name;
        public int mediaID;
        public int templateFieldID;
        public Widget widget;
        public Field(string name) {
            this.name = name;
        }

        public Field() { }

        static public Predicate<Field> byName(string name) {
            return delegate(Field t) {
                return t.name == name;
            };
        }
    }
}
