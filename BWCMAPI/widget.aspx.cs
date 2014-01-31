using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using BWCMAPI.data;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;

namespace BWCMAPI {
    public partial class widgetrender : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs ea) {
            Dictionary<string, Object> response = new Dictionary<string, Object>();
            NameValueCollection q = Request.QueryString;
            HttpStatusCode result = HttpStatusCode.OK;

            try {
                switch (Request.PathInfo) {
                    case "/refresh":
                        // intended to be called every 5 minutes to refresh widgets and update images
                        //Scala.garbageCollection();

                        int count = 0;
                        List<Widget> removeList = new List<Widget>();
                        foreach (Widget wi in Widget.renderList) {
                            if (!Scala.mediaReferenced(wi.mediaID)) {
                                removeList.Add(wi);
                            } else if (wi.needRefresh() || q["all"] != null) {
                                count++;
                                wi.refresh(); // render and upload new version
                            }
                        }
                        if (removeList.Count > 0) {
                            foreach (Widget wi in removeList) {
                                Global.d("expiring non-referenced widget " + wi.mediaID + ": " + wi.GetType().ToString());
                                Widget.renderList.Remove(wi);
                            }
                            Global.dataDirty = true;
                        }
                        response.Add("count", count);
                        break;

                    case "/preview":
                        require(q, "widget");
                        Widget w = (Widget)Global.json.DeserializeObject(q["widget"]);
                        byte[] data = w.render();

                        Response.ContentType = "image/png";
                        Response.Clear();
                        Response.OutputStream.Write(data, 0, data.Length);
                        return; // don't do normal result processing

                    default:
                        response.Add("error", "malformed request");
                        result = HttpStatusCode.NotFound;
                        break;
                }
            } catch (ArgumentException e) {
                result = HttpStatusCode.BadRequest;
                response.Add("message", e.Message);
            } catch (Exception e) {
                d(e.ToString());
                response.Add("error", "Exception while handling request: " + e.Message);
                result = HttpStatusCode.InternalServerError;
            }
            response.Add("code", result);
            Response.StatusCode = (int)result;
            Response.ContentType = "text/json";
            Response.Write(Global.json.Serialize(response));
        }

        private void require(NameValueCollection q, params string[] key) {
            for (int i = 0; i < key.Length; i++) {
                if (q[key[i]] == null) throw new ArgumentException("missing required argument: " + key[i]);
            }
        }

        private void d(string msg) {
            Global.d(msg);
        }
    }
}
