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
using ScalaWS.Playlist;

namespace BWCMAPI.data {
    public class Slide {
        // corresponds to a Scala message
        public int id;
        public string name;
        public int templateID;
        public bool changed = false;
        public List<Field> fields = new List<Field>();

        public string startDate, stopDate;
        public string startTime, stopTime;
        public List<string> days;

        public Slide(int id, string name) {
            this.id = id; this.name = name;
        }

        public Slide() { this.id = -1;  }

        public weekdayEnum?[] scalaDays() {
            this.days = new List<string>() { "sun", "mon", "tue", "wed", "thu", "fri", "sat" }; // override, don't allow editing days

            List<weekdayEnum?> scala = new List<weekdayEnum?>();
            foreach (string day in days) {
                switch (day) {
                    case "sun": scala.Add(weekdayEnum.SUNDAY); break;
                    case "mon": scala.Add(weekdayEnum.MONDAY); break;
                    case "tue": scala.Add(weekdayEnum.TUESDAY); break;
                    case "wed": scala.Add(weekdayEnum.WEDNESDAY); break;
                    case "thu": scala.Add(weekdayEnum.THURSDAY); break;
                    case "fri": scala.Add(weekdayEnum.FRIDAY); break;
                    case "sat": scala.Add(weekdayEnum.SATURDAY); break;
                }
            }
            return scala.ToArray();
        }

        static public Predicate<Slide> byID(int id) {
            return delegate(Slide t) {
                return t.id == id;
            };
        }
    }
}
