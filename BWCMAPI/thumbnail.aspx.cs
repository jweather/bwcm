using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Drawing;

namespace BWCMAPI {
    public partial class thumbnail : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            NameValueCollection q = Request.QueryString;

            byte[] thumbdata;
            try {
                int mediaID = Convert.ToInt32(q["id"]);

                if (q["big"] == "1") {
                    thumbdata = Scala.largeThumbnail(mediaID);
                } else {
                    thumbdata = Scala.thumbnail(mediaID);
                }
            } catch {
                // generate placeholder image
                if (q["placeholder"] != null) {
                    int W = 224, H = 115;
                    Bitmap image = new Bitmap(W, H);
                    Graphics g = Graphics.FromImage(image);
                    g.Clear(Color.LightGray);


                    fitText(g, Color.Gray, "Calibri", q["placeholder"], new RectangleF(0, 0, W, H));
                    /*
                    Font f = new Font("Calibri", 24);
                    SizeF size = g.MeasureString(q["placeholder"], f);
                    g.DrawString(q["placeholder"], f, new SolidBrush(Color.Gray), (W-size.Width)/2, (H-size.Height)/2);
                     */

                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    thumbdata = ms.ToArray();

                } else {
                    thumbdata = File.ReadAllBytes(Global.cfg("defaultThumb"));
                }
            }
            Response.ContentType = "image/png";
            Response.Clear();
            Response.OutputStream.Write(thumbdata, 0, thumbdata.Length);
        }
        // fit text in box
        public void fitText(Graphics g, Color c, string fontFamily, string text, RectangleF fit) {
            SizeF textSize;
            Font font;
            float pt = 128;
            while (true) {
                font = new Font(fontFamily, pt);
                textSize = g.MeasureString(text, font, (int)fit.Width);
                if (textSize.Height < fit.Height)
                    break;
                pt = pt - pt / 10;
                if (pt <= 8) break;
            }

            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.LineAlignment = StringAlignment.Center;
            g.DrawString(text, font, new SolidBrush(c), fit, format);
        }
    }
}
