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
using System.Threading;

namespace BWCMAPI {
    public partial class api : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs ea) {
            Dictionary<string, Object> response = new Dictionary<string, Object>();
            NameValueCollection q = Request.QueryString;
            HttpStatusCode result = HttpStatusCode.OK;
            byte[] postdata = null;
            string body = null;
            if (Request.RequestType == "POST") {
                postdata = new byte[Request.ContentLength];
                Request.InputStream.Read(postdata, 0, Request.ContentLength);
                if (Request.PathInfo != "/upload")
                    body = Encoding.ASCII.GetString(postdata, 0, Request.ContentLength);
            }
            try {
                BUser user = (BUser)Session["user"], u;
                bool checkUser = false, checkAdmin = false;
                switch (Request.PathInfo) {
                    case "/info":
                        // check if user is logged in
                        response.Add("user", Session["user"]);
                        break;
                    case "/login":
                        require(q, "user", "pass");
                        user = BUser.find(q["user"]);
                        if (user == null) {
                            response.Add("error", "No such user name or password.");
                            Global.audit("<anon>", "login failure for " + q["user"]);
                        } else {
                            if (!user.checkPassword(q["pass"])) {
                                response.Add("error", "No such user name or password.");
                                Global.audit("<anon>", "password mismatch for " + q["user"]);
                                user = null;
                            } else {
                                Session["user"] = user;
                                audit("logged in");
                            }
                        }
                        break;
                    case "/logout":
                        audit("logged out");
                        Session["user"] = null;
                        break;
                    default:
                        checkUser = true;
                        break;
                }
                if (checkUser && user == null) {
                    result = HttpStatusCode.Forbidden;
                } else if (checkUser) {
                    switch (Request.PathInfo) {
                        case "/players":
                            List<Player> players = Scala.getPlayers(user);
                            response.Add("result", players);
                            break;
                        case "/players/update":
                            Player pl = (Player)Global.json.DeserializeObject(body);
                            if (!checkPriv(user, pl.name)) {
                                audit("403 /players/update " + pl.name);
                                result = HttpStatusCode.Forbidden;
                            } else {
                                string changes = Scala.savePlayer(pl);
                                audit("updated " + pl.name + changes);
                                response.Add("result", pl); // return player with IDs filled in
                            }
                            break;
                        case "/templates":
                            response.Add("result", Scala.templates);
                            break;

                        case "/upload":
                            string player = Request.Headers["X_PLAYER"];
                            string fname = Request.Headers["X_FILENAME"].Replace(@"\", "");
                            fname = Path.GetFileNameWithoutExtension(fname) + "-" + Guid.NewGuid().ToString() + Path.GetExtension(fname);
                            if (!checkPriv(user, player)) {
                                audit("403 /upload " + player);
                                result = HttpStatusCode.Forbidden;
                            } else {
                                string localPath = Global.cfg("uploadDir") + @"\" + fname;

                                FileStream fs = File.Open(localPath, FileMode.Create);
                                fs.Write(postdata, 0, Request.ContentLength);
                                fs.Close();

                                int mediaID = Scala.uploadMedia(localPath, "/" + player, fname);
                                response.Add("id", mediaID.ToString());

                                File.Delete(localPath);
                                audit("uploaded " + fname + " for " + player);
                            }
                            break;

                        case "/passwd":
                            require(q, "pass");
                            user.setPassword(q["pass"]);
                            audit("changed password");
                            Global.dataDirty = true;
                            break;

                        default:
                            checkAdmin = true;
                            break;
                    }
                }

                if (checkAdmin && (user == null || !user.isAdmin)) {
                    result = HttpStatusCode.Forbidden;
                } else if (checkAdmin) {
                    switch (Request.PathInfo) {
                        case "/users":
                            response.Add("result", BUser.users);
                            break;
                        case "/users/update": // also creates users
                            u = (BUser)Global.json.DeserializeObject(body);
                            if (u.user == "") throw new ArgumentException("no username specified");
                            u.save();
                            Global.dataDirty = true;
                            audit("create/update user " + u.user);
                            break;
                        case "/users/delete":
                            require(q, "user");
                            u = BUser.find(q["user"]);
                            u.delete();
                            Global.dataDirty = true;
                            audit("delete user " + u.user);
                            break;

                        case "/crawl":
                            // try to go back to multiline formatting
                            string crawl = Scala.crawlText();
                            crawl = crawl.Replace("          ", "\n");
                            response.Add("result", crawl);
                            break;
                        case "/crawl/update":
                            string text = q["crawl"];
                            // convert newlines to gaps in text crawl
                            text = text.Replace("\n", "          ");

                            Scala.updateCrawl(text);
                            audit("update crawl: " + text.Substring(0, Math.Min(text.Length, 60)));
                            break;

                        // debug
                        case "/reload":
                            Persistence.loadData();
                            Scala.loadData();
                            break;

                        default:
                            response.Add("error", "malformed request");
                            result = HttpStatusCode.NotFound;
                            break;
                    }
                }
            } catch (ArgumentException e) {
                result = HttpStatusCode.BadRequest;
                response.Add("message", e.ToString());
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

        private bool checkPriv(BUser user, string player) {
            if (user.isAdmin) return true;
            foreach (Player p in Scala.getPlayers(user)) {
                if (p.name == player) return true;
            }
            return false;
        }

        private void d(string msg) {
            Global.d(msg);
        }

        private void audit(string msg) {
            if (Session["user"] == null) return;
            BUser u = (BUser)Session["user"];
            Global.audit(u.user, msg);
        }
    }
}
