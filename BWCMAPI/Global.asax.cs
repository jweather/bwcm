using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using log4net;
using log4net.Config;
using System.Web.Script.Serialization;
using BWCMAPI.data;
using System.Timers;
using System.Web.Hosting;

namespace BWCMAPI {
    public class Global : System.Web.HttpApplication {
        public static ILog log;
        public static JavaScriptSerializer json;
        public static bool dataDirty = false;

        protected void Application_Start(object sender, EventArgs e) {
            log = LogManager.GetLogger(typeof(Global));
            XmlConfigurator.Configure();
            d("startup");

            json = new JavaScriptSerializer(new Types());

            Persistence.loadData();
            Scala.loadData();

            // write data to disk periodically
            Timer t = new Timer(5 * 1000);
            t.Elapsed += new ElapsedEventHandler(checkpoint_Elapsed);
            t.Start();
        }

        void checkpoint_Elapsed(object sender, ElapsedEventArgs e) {
            if (dataDirty)
                Persistence.saveData();
        }

        protected void Session_Start(object sender, EventArgs e) {

        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            string url = Request.Url.AbsolutePath;
            string query = Request.Url.Query;
            if (url.StartsWith("/api/")) {
                if (query.StartsWith("?")) query = query.Substring(1);
                Context.RewritePath("/api.aspx", url.Replace("/api", ""), query);
            } else if (url == "/") {
                Context.RewritePath("/index.html");
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {

        }

        protected void Application_Error(object sender, EventArgs e) {

        }

        protected void Session_End(object sender, EventArgs e) {

        }

        protected void Application_End(object sender, EventArgs e) {

        }

        public static string cfg(string key) {
            string value = ConfigurationSettings.AppSettings[key];
            if (value == null)
                throw new Exception("Unknown config key referenced: " + key);
            return value;
        }

        public static string cfg(string key, string def) {
            string value = ConfigurationSettings.AppSettings[key];
            if (value == null)
                return def;
            return value;
        }            

        public static void d(string msg) {
            log.Debug(msg);
        }

        public static void error(string msg) {
            log.Error(msg);
        }

        public static void audit(string user, string msg) {
            log.Info(user + ": " + msg);
        }

        public static string appData(string p) {
            return HostingEnvironment.MapPath("~/App_Data/" + p);
        }
    }
}