﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Xml;
using LinqToTwitter;

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
            int marginX = 16, marginY = 8;
            SizeF textSize;
            Font font;
            // fit text in box
            while (true) {
                font = new Font("Calibri", pt);
                textSize = g.MeasureString(text, font, W - marginX * 2);
                if (textSize.Height < H - marginY*2)
                    break;
                pt = pt - pt/10;
                if (pt <= 8) break;
            }
            g.DrawImage(new Bitmap(HostingEnvironment.MapPath("~/App_Data/1059x94 text.png")), new Rectangle(0, 0, W, H));

            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.LineAlignment = StringAlignment.Center;
            g.DrawString(text, font, new SolidBrush(Color.White), new RectangleF(marginX, marginY, W-marginX*2, H-marginY*2), format);
        }
        protected override Size getSize() {
            return new Size(1059, 94);
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
            return new Size(1059, 94);
        }
        
        protected override void render(Graphics g) {
            int W = getSize().Width, H = getSize().Height;

            g.DrawImage(new Bitmap(HostingEnvironment.MapPath("~/App_Data/1059x94 twitter.png")), new Rectangle(0, 0, W, H));

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

            Font font = new Font("Calibri", 14);
            string text = "";
            foreach (Status tweet in tweets) {
                text = tweet.ScreenName + ": " + tweet.Text.Replace("\n", " ");
                break;
            }

            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.LineAlignment = StringAlignment.Center;
            g.DrawString(text, font, new SolidBrush(Color.White), new RectangleF(100, 16, 945, 54), format);
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

            g.DrawImage(new Bitmap(HostingEnvironment.MapPath("~/App_Data/423x423 weather bg.png")), new Rectangle(0, 0, W, H));

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

                g.DrawImage(b, 250, 70);

            } catch (Exception e) {
                Global.d("WX exception: " + e.ToString());
            }

            Font tfont = new Font("Futura", 128);
            Font cfont = new Font("Calibri", 28);

            Brush brush = new SolidBrush(Color.FromArgb(100, 100, 100));
            g.DrawString(temp, tfont, brush, 40, 240);

            SizeF csize = g.MeasureString(cond, cfont);
            g.DrawString(cond, cfont, brush, 250 + (128 - csize.Width) / 2, 190);

            //g.DrawString("Updated at " + DateTime.Now.ToShortTimeString(), cfont, brush, 40, 280);
        }

        protected override Size getSize() {
            return new Size(423, 423);
        }

        protected override string key() {
            return "weather"; // singleton
        }
        public override bool needRefresh() {
            return true; // todo check time since last refresh
        }
    }

    public class WidgetNone : Widget {
        public WidgetNone() { }

        protected override void render(Graphics g) {
            g.Clear(Color.White);
            Global.error("Why are you rendering a WidgetNone?");
        }
        protected override Size getSize() {
            return new Size(100, 100);
        }
        protected override string key() {
            return "empty";
        }
        public override bool needRefresh() {
            return false;
        }
    }
}
