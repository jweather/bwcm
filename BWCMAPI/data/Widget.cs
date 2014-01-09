using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using LinqToTwitter;
using System.Net;
using System.Xml;
using System.Drawing.Drawing2D;
using System.Web.Hosting;

namespace BWCMAPI.data {
    public abstract class Widget {
        public int mediaID;
        public static List<Widget> renderList = new List<Widget>();

        public string[] types() {
            return new string[] { "text", "twitter", "weather" };
        }

        public byte[] render() {
            Size size = getSize();
            Bitmap image = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(Color.Blue);

            render(g);

            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        // commit a new widget
        public int upload() {
            string key = this.key();
            Widget existing = renderList.Find(Widget.byKey(key));
            if (existing != null)
                return existing.mediaID;
            
            int mediaID = refresh();

            renderList.Add(this);
            Global.dataDirty = true;
            return mediaID;
        }

        // render and upload
        public int refresh() {
            string fname = key() + ".png";
            foreach (char c in Path.GetInvalidFileNameChars())
                fname = fname.Replace(c, '_');

            string localFile = Global.cfg("uploadDir") + @"\" + fname;

            try { File.Delete(localFile); } catch { }
            byte[] data = render();
            File.WriteAllBytes(localFile, data);

            mediaID = Scala.uploadMedia(localFile, "/Widgets", fname);
            try { File.Delete(localFile); } catch { }
            return mediaID;
        }

        abstract protected void render(Graphics g);
        abstract protected Size getSize();
        abstract protected string key();
        abstract public bool needRefresh();

        static public Predicate<Widget> byMediaID(int id) {
            return delegate(Widget t) {
                return t.mediaID == id;
            };
        }
        static public Predicate<Widget> byKey(string key) {
            return delegate(Widget t) {
                return t.key() == key;
            };
        }
    }

    public class WidgetText : Widget {
        public string text;
        public WidgetText() { }

        protected override void render(Graphics g) {
            int W = getSize().Width, H = getSize().Height;
            float pt = 128;
            SizeF textSize;
            Font font;
            while (true) {
                font = new Font("Calibri", pt);
                textSize = g.MeasureString(text, font, W - 20);
                if (textSize.Height < H - 20)
                    break;
                pt = pt - pt/10;
                if (pt <= 8) break;
            }
            g.DrawString(text, font, new SolidBrush(Color.White), new RectangleF((W - textSize.Width) / 2, (H - textSize.Height) / 2, W-20, H-20));
        }
        protected override Size getSize() {
            return new Size(499, 666);
        }
        protected override string key() {
            return "text " + text;
        }
        public override bool needRefresh() {
            return false;
        }
    }

    public class WidgetTwitter : Widget {
        public string handles;
        public WidgetTwitter() { }

        protected override Size getSize() {
            return new Size(499, 666);
        }
        
        protected override void render(Graphics g) {
            int W = getSize().Width, H = getSize().Height;
            int margin = 20;
            g.Clear(Color.Bisque);

            if (handles == null) return;

            var auth = new ApplicationOnlyAuthorizer();
            auth.Credentials = new InMemoryCredentials { // @weatherfnord/multitweeterer
                ConsumerKey = "vTTLuEldRiVwqdBv97q6Q",
                ConsumerSecret = "4pLpDAjDAqovaY4DzQW7wj8ow9MeMmJ10nLlVED5zk"
            };
            auth.Authorize();
            var ctx = new TwitterContext(auth);
            var res =
                from search in ctx.Search
                where search.Type == SearchType.Search &&
                    search.Query == "LINQ to Twitter"
                select search;

            Search srch = res.Single();

            IEnumerable<Status> tweets = Enumerable.Empty<Status>();
            foreach (string h in handles.Split()) {
                string handle = h;
                if (h[0] == '@') handle = handle.Substring(1);

                try {
                    var response =
                        (from tweet in ctx.Status
                         where tweet.Type == StatusType.User &&
                               tweet.ScreenName == handle &&
                               tweet.Count == 5
                         select tweet)
                        .ToList();
                    tweets = tweets.Concat(response);
                } catch { }
            }
            tweets = tweets.OrderByDescending(item => item.CreatedAt);

            Font font = new Font("Calibri", 18);
            SizeF textSize;
            string text = "";

            foreach (Status tweet in tweets) {
                string temp = "";
                if (text != "") temp = text + "\n\n";
                temp += tweet.ScreenName + ": " + tweet.Text;

                textSize = g.MeasureString(temp, font, W - margin*2);
                if (textSize.Height > H - margin*2) break; // no more room
                text = temp;
            }


            g.DrawString(text, font, new SolidBrush(Color.Black), new RectangleF(margin, margin, W - margin*2, H - margin*2));
        }

        protected override string key() {
            return "twitter " + handles;
        }
        public override bool needRefresh() {
            return true; // todo check time since last refresh
        }
    }

    public class WidgetWeather : Widget {
        public WidgetWeather() { }

        protected override void render(Graphics g) {
            int W = getSize().Width, H = getSize().Height;

            g.Clear(Color.Gray);

            string temp = "", cond = "";
            try {
                WebRequest req = WebRequest.Create("http://weather.yahooapis.com/forecastrss?w=12776569"); // WOEID for zip 44017
                StreamReader sr = new StreamReader(req.GetResponse().GetResponseStream());
                string resp = sr.ReadToEnd();
                sr.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resp);
                XmlNamespaceManager man = new XmlNamespaceManager(doc.NameTable);
                man.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

                XmlNode now = doc.SelectSingleNode("/rss/channel/item/yweather:condition", man);
                temp = now.Attributes["temp"].Value + "°";
                cond = now.Attributes["text"].Value;

                int code = 3200; // not available
                try {
                    code = Convert.ToInt32(now.Attributes["code"].Value);
                } catch { }

                // draw wxicon
                string path = HostingEnvironment.MapPath("~/App_Data/wxicons/" + code + ".png");
                if (!File.Exists(path))
                    path = HostingEnvironment.MapPath("~/App_Data/wxicons/3200.png");
                Bitmap b = new Bitmap(path);

                g.DrawImage(b, 250, 50);

            } catch (Exception e) {
                Global.d("WX exception: " + e.ToString());
            }

            Font tfont = new Font("Calibri", 72);
            Font cfont = new Font("Calibri", 28);

            Brush brush = new SolidBrush(Color.White);
            g.DrawString(temp, tfont, brush, 40, 40);

            SizeF csize = g.MeasureString(cond, cfont);
            g.DrawString(cond, cfont, brush, 250 + (128 - csize.Width) / 2, 190);

            //g.DrawString("Updated at " + DateTime.Now.ToShortTimeString(), cfont, brush, 40, 280);
        }

        protected override Size getSize() {
            return new Size(499, 666); // todo, not determined yet
        }

        protected override string key() {
            return "weather"; // singleton
        }
        public override bool needRefresh() {
            return true; // todo check time since last refresh
        }
    }
}
