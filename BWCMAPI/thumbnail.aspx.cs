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
                thumbdata = Scala.thumbnail(mediaID);
            } catch {
                // generate placeholder image
                if (q["placeholder"] != null) {
                    int W = 224, H = 115;
                    Bitmap image = new Bitmap(W, H);
                    Graphics g = Graphics.FromImage(image);
                    g.Clear(Color.LightGray);

                    Font f = new Font("Calibri", 24);
                    SizeF size = g.MeasureString(q["placeholder"], f);
                    g.DrawString(q["placeholder"], f, new SolidBrush(Color.Gray), (W-size.Width)/2, (H-size.Height)/2);

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
    }
}
