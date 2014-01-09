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
    public class Template {
        public int id;
        public string name;
        public List<Field> fields = new List<Field>();
        public TemplateInfo info;

        public Template(int id, string name, string description) {
            this.id = id;
            this.name = name.Replace(".scb", "");
            try {
                info = Global.json.Deserialize<TemplateInfo>(description);
            } catch (Exception e) {
                Global.error("Failed to parse TemplateInfo for " + name + ": " + e.Message);
            }
        }

        static public Predicate<Template> byID(int id) {
            return delegate(Template t) {
                return t.id == id;
            };
        }

        static public Predicate<Template> byName(string name) {
            return delegate(Template t) {
                return t.name == name;
            };
        }
    }
}
