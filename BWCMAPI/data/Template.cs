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
using System.IO;

namespace BWCMAPI.data {
    public class Template {
        public int id;
        public string name;
        public List<Field> fields = new List<Field>();
        public TemplateInfo info;

        public Template(int id, string name) {
            this.id = id;
            this.name = name.Replace(".scb", "");
            try {
                string blob = File.ReadAllText(Global.appData(this.name + ".json"));
                info = Global.json.Deserialize<TemplateInfo>(blob);
            } catch (Exception e) {
                Global.error("Failed to parse TemplateInfo for " + name + ": " + e.Message);
            }
        }

        public void sortFields() {
            List<Field> reorderFields = new List<Field>();
            foreach (FieldInfo fi in info.fields)
                reorderFields.Add(fields.Find(Field.byName(fi.n)));
            fields = reorderFields;
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
