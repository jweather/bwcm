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
    public class PlayerInfo {
        public int defaultDuration;
        public string defaultTwitter;

        public static bool operator ==(PlayerInfo a, PlayerInfo b) {
            if (System.Object.ReferenceEquals(a, null) && System.Object.ReferenceEquals(b, null)) return true;
            if (System.Object.ReferenceEquals(a, null) || System.Object.ReferenceEquals(b, null)) return false;

            if (a.defaultDuration != b.defaultDuration) return false;
            if (a.defaultTwitter != b.defaultTwitter) return false;
            return true;
        }

        public static bool operator !=(PlayerInfo a, PlayerInfo b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
