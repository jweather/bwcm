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
using System.Web.Script.Serialization;

namespace BWCMAPI.data {
    public class Types : JavaScriptTypeResolver {
        static private Dictionary<string, Type> types = new Dictionary<string, Type>();
        static private Dictionary<Type, string> typekeys = new Dictionary<Type,string>();

        static Types() {
            types.Add("user", typeof(BUser));
            types.Add("player", typeof(Player));
            types.Add("slide", typeof(Slide));
            types.Add("template", typeof(Template));
            types.Add("field", typeof(Field));
            types.Add("widget", typeof(Widget));
            types.Add("text", typeof(WidgetText));
            types.Add("none", typeof(WidgetNone));
            types.Add("twitter", typeof(WidgetTwitter));
            types.Add("weather", typeof(WidgetWeather));
            types.Add("templateinfo", typeof(TemplateInfo));
            types.Add("fieldinfo", typeof(FieldInfo));
            foreach (String key in types.Keys) {
                typekeys.Add(types[key], key);
            }

        }
        public override Type ResolveType(string id) {
            return types[id];
        }

        public override string ResolveTypeId(Type type) {
            return typekeys[type];
        }
    }
}
