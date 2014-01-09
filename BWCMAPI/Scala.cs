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
using BWCMAPI.data;
using ScalaWS;
using ScalaWS.Playlist;
using ScalaWS.Media;
using ScalaWS.Template;
using ScalaWS.Message;
using System.Text;
using ScalaWS.UploadFile;
using System.Net;
using System.IO;
using System.Diagnostics;
using ScalaWS.Thumbnail;
using System.Web.Caching;

namespace BWCMAPI {
    public class Scala {
        // todo handle refreshing data and server being offline
        public static List<Player> players = new List<Player>();
        public static List<Template> templates = new List<Template>();

        // Scala endpoints
        static playlistService playlistServ = new playlistService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));
        static mediaService mediaServ = new mediaService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));
        static templateService templateServ = new templateService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));
        static messageService messageServ = new messageService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));
        static uploadfileService uploadServ = new uploadfileService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));

        // Scala data
        static int managedCatID;
        static messageDataFieldTO crawlTextField;

        // thumbnail cache
        static ThumbCache thumbCache = new ThumbCache(8 * 1024 * 1024);

        // get groups and player names from Scala
        public static void loadData() {
            Global.d("Scala.loadData");
            players.Clear();
            templates.Clear();

            try {
                // find category
                ScalaWS.Playlist.searchCriteriaTO[] plcrit = new ScalaWS.Playlist.searchCriteriaTO[1]; plcrit[0] = new ScalaWS.Playlist.searchCriteriaTO();
                plcrit[0].column = "name"; plcrit[0].value = Global.cfg("catManaged");
                plcrit[0].restriction = ScalaWS.Playlist.restrictionEnum.EQUALS; plcrit[0].restrictionSpecified = true;
                ScalaWS.Playlist.categoryTO[] cats = playlistServ.listCategories(plcrit, null);
                if (cats.Length != 1) throw new Exception("Did not find managed category named '" + Global.cfg("catManaged") + "' (Web.config:catManaged)");
                managedCatID = cats[0].id;

                plcrit[0].column = "categoryId"; plcrit[0].value = managedCatID.ToString();
                ScalaWS.Playlist.playlistTO[] playlists = playlistServ.list(plcrit, null);
                foreach (playlistTO playlist in playlists) {
                    Player player = new Player(playlist.id, playlist.name);
                    players.Add(player);
                    ScalaWS.Playlist.playlistItemTO[] items = playlistServ.getPlaylistItems(playlist.id, true);
                    foreach (ScalaWS.Playlist.playlistItemTO item in items) {
                        try {
                            // if (item.playlistItemType != playlistItemTypeEnum.MESSAGE) continue; // not supported by ScalaWS, hopefully no non-message items show up in this playlist
                            // item.mediaID is the message ID, item.id is the playlistItemID
                            messageTO message = messageServ.get(item.mediaId, true);
                            Slide slide = new Slide(item.mediaId, message.name);
                            player.slides.Add(slide);

                            slide.templateID = message.templateId;

                            messageDataFieldTO[] fields = messageServ.getFiles(item.mediaId, true);
                            foreach (messageDataFieldTO fieldTO in fields) {
                                Field field = new Field(fieldTO.name);
                                field.mediaID = Convert.ToInt32(fieldTO.value);
                                field.widget = Widget.renderList.Find(Widget.byMediaID(field.mediaID));

                                slide.fields.Add(field);
                            }
                        } catch (Exception e) {
                            Global.d("Failed to load playlist item in " + player.name + ": " + e);
                        }
                    }
                }

                // get templates in master category
                ScalaWS.Template.searchCriteriaTO[] tcrit = new ScalaWS.Template.searchCriteriaTO[1]; tcrit[0] = new ScalaWS.Template.searchCriteriaTO();
                tcrit[0].column = "categoryId"; tcrit[0].value = managedCatID.ToString();
                tcrit[0].restriction = ScalaWS.Template.restrictionEnum.EQUALS; tcrit[0].restrictionSpecified = true;

                templateTO[] temps = templateServ.list(tcrit, null);
                foreach (templateTO templateTO in temps) {
                    try {
                        Template template = new Template(templateTO.id, templateTO.name, templateTO.description);
                        templateDataFieldTO[] fields = templateServ.getFiles(template.id, true);
                        foreach (templateDataFieldTO fieldTO in fields) {
                            if (fieldTO.type != templateFieldTypeEnum.IMAGE) continue;
                            Field field = new Field(fieldTO.name);
                            field.templateFieldID = fieldTO.id;
                            template.fields.Add(field);
                        }
                        templates.Add(template);
                    } catch (Exception e) {
                        Global.d("Failed to load template data: " + e);
                    }
                }

                // get crawl message text
                ScalaWS.Message.searchCriteriaTO[] mcrit = new ScalaWS.Message.searchCriteriaTO[1]; mcrit[0] = new ScalaWS.Message.searchCriteriaTO();
                mcrit[0].column = "name"; mcrit[0].value = Global.cfg("crawlMessage");
                mcrit[0].restriction = ScalaWS.Message.restrictionEnum.EQUALS; mcrit[0].restrictionSpecified = true;

                messageTO[] msgs = messageServ.list(mcrit, null);
                if (msgs.Length == 0)
                    throw new Exception("Cannot find crawl message named '" + Global.cfg("crawlMessage") + "'");
                messageTO msg = msgs[0];
                messageDataFieldTO[] f = messageServ.getFields(msg.id, true);
                crawlTextField = f[0];

            } catch (Exception e) {
                Global.d("Scala.loadData: " + e);
            }
                
            Global.d("end Scala.loadData: " + players.Count + " players and " + templates.Count + " templates loaded.");
        }

        // can also use thumbnail service to wait for thumbnail generation to complete
        public static byte[] thumbnail(int id) {
            if (thumbCache.ContainsKey(id))
                return thumbCache[id];

            string data = mediaServ.getThumbnailAsPNGBytes(id, true);
            byte[] decode = Convert.FromBase64String(data);
            thumbCache[id] = decode;
            return decode;
        }

        public static List<Player> getPlayers(BUser user) {
            if (user.isAdmin) {
                return players;
            }
            List<Player> results = new List<Player>();
            foreach (Player p in players) {
                if (user.players.Contains(p.name)) {
                    results.Add(p);
                }
            }
            return results;
        }

        public static string savePlayer(Player player) {
            string summary = "";
            // commit changes to Scala playlist and update local data
            Player old = players.Find(Player.byID(player.id));
            if (old == null) {
                Global.d("Scala.savePlayer got nonexistent player ID " + player.id);
                throw new ArgumentException("player ID " + player.id + " doesn't exist");
            }

            // save changes to Scala
            List<int> oldIDs = new List<int>();
            List<int> replaced = new List<int>();
            foreach (Slide s in old.slides) oldIDs.Add(s.id);

            // create new messages
            foreach (Slide slide in player.slides) {
                if (!slide.changed) {
                    oldIDs.Remove(slide.id);
                    continue;
                } else {
                    replaced.Add(slide.id);
                }

                // create new message
                Template template = templates.Find(Template.byID(slide.templateID));
                messageTO message = new messageTO();
                message.name = slide.name; message.templateId = slide.templateID; message.templateIdSpecified = true;
                messageDataFieldTO[] fields = new messageDataFieldTO[slide.fields.Count];
                for (int f=0; f < slide.fields.Count; f++) {
                    Field field = slide.fields[f];
                    Field tfield = template.fields.Find(Field.byName(field.name));
                    if (tfield == null) continue; // no such field in template

                    fields[f] = new messageDataFieldTO();
                    fields[f].id = tfield.templateFieldID; fields[f].idSpecified = true; // use template field ID
                    fields[f].name = field.name;

                    if (field.widget != null)
                        field.mediaID = field.widget.upload();

                    fields[f].value = field.mediaID.ToString();
                }
                message.approvalStatusSpecified = true;
                message.approvalStatus = null;
                message = messageServ.create(message, fields);
                if (oldIDs.Contains(slide.id))
                    summary += " -- updated " + slide.id + "=>" + message.id + ": " + message.name;
                else
                    summary += " -- added " + message.id + ": " + message.name;

                setApproval(message.id);
                messageServ.addCategory(message.id, true, managedCatID, true);

                // update IDs
                slide.id = message.id;
                slide.changed = false;
            }

            foreach (int id in oldIDs) {
                // deleted or replaced
                messageServ.delete(id, true);
                thumbCache.Remove(id);
                if (!replaced.Contains(id))
                    summary += " -- deleted slide " + id;
            }

            // reorder messages in playlist
            ScalaWS.Playlist.playlistItemTO[] items = new ScalaWS.Playlist.playlistItemTO[player.slides.Count];
            for (int s=0; s < player.slides.Count; s++) {
                Slide slide = player.slides[s];
                items[s] = new ScalaWS.Playlist.playlistItemTO();
                items[s].mediaId = slide.id; items[s].mediaIdSpecified = true;
            }
            playlistServ.deleteAllPlaylistItems(player.id, true);
            playlistServ.addPlaylistItems(player.id, true, items);

            // save changes locally
            players.Remove(old);
            players.Add(player);

            if (summary == "") summary = " -- reordered slides";
            return summary;
        }

        public static int uploadMedia(string localFile, string mediaPath, string mediaName) {
            if (!mediaPath.StartsWith("/")) mediaPath = "/" + mediaPath;
            requestFileTO req = new requestFileTO();
            req.filename = mediaName;
            req.path = "/content" + mediaPath;
            req.type = uploadTypeEnum.MEDIA;
            req.typeSpecified = true;
            req.size = new FileInfo(localFile).Length;
            req.sizeSpecified = true;
            fileUploadTO up = uploadServ.requestUpload(req);

            // uploads require basic auth for the upload servlet
            string auth = Global.cfg("scalaUser") + ":" + Global.cfg("scalaPass");
            byte[] binaryData = new Byte[auth.Length];
            binaryData = System.Text.Encoding.UTF8.GetBytes(auth);
            auth = Convert.ToBase64String(binaryData);
            auth = "Basic " + auth;

            WebClient cli = new WebClient();
            cli.Headers["AUTHORIZATION"] = auth;
            cli.Headers["filenameWithPath"] = Global.cfg("scalaNetwork") + "/content" + mediaPath + "/" + up.uploadAsFilename;
            
            try {
                cli.UploadFile(Global.cfg("scalaURL") + "/servlet/uploadFile", localFile);
            } catch (WebException e) {
                int code = 0;
                HttpWebResponse r = (HttpWebResponse)e.Response;
                if (r != null) code = (int)r.StatusCode;
                if (code == 400)
                    throw new Exception("Incorrect Scala network name configured, check configuration");
                throw new Exception("Failed to connect to Scala web services, check URL: " + code);
            }

            uploadServ.uploadFinished(up.fileId, true);
            setApproval(up.mediaItemId);
            mediaServ.addCategory(up.mediaItemId, true, managedCatID, true);

            return up.mediaItemId;
        }

        private static void setApproval(int id) {
            ScalaWS.Media.approvalStatusTO approval = mediaServ.getApproval(id, true);
            approval.approvalStatus = ScalaWS.Media.approvalStatusEnum.APPROVED;
            approval.approvalStatusSpecified = true;
            approval.user = "administrator";
            approval.editTimestamp = DateTime.Now;
            approval.editTimestampSpecified = true;
            mediaServ.updateApproval(id, true, approval);
        }

        private static long FileInfo(string localFile) {
            throw new NotImplementedException();
        }

        public static string crawlText() {
            return crawlTextField.value;
        }

        public static void updateCrawl(string p) {
            crawlTextField.value = p;
            messageServ.updateField(crawlTextField);
        }

        private static HashSet<int> referencedMedia = null;
        public static void garbageCollection() {
            // visit all BWCM playlists, record all message IDs and media IDs for field values
            HashSet<int> references = new HashSet<int>();
            foreach (Player p in players) {
                foreach (Slide s in p.slides) {
                    references.Add(s.id);
                    foreach (Field f in s.fields) {
                        references.Add(f.mediaID);
                    }
                }
            }

            // list all BWCM messages and remove unused
            ScalaWS.Message.searchCriteriaTO[] mcrit = new ScalaWS.Message.searchCriteriaTO[1]; mcrit[0] = new ScalaWS.Message.searchCriteriaTO();
            mcrit[0].column = "categoryId"; mcrit[0].value = managedCatID.ToString();
            mcrit[0].restriction = ScalaWS.Message.restrictionEnum.EQUALS; mcrit[0].restrictionSpecified = true;

            int msgcount = 0;
            messageTO[] messages = messageServ.list(mcrit, null);
            foreach (messageTO msg in messages) {
                if (references.Contains(msg.id)) {
                    msgcount++;
                    continue;
                }
                Global.d("marked message " + msg.id + ": " + msg.name + " for deletion");
                messageServ.delete(msg.id, true);
            }

            // list all BWCM media items and remove unused
            ScalaWS.Media.searchCriteriaTO[] fcrit = new ScalaWS.Media.searchCriteriaTO[1]; fcrit[0] = new ScalaWS.Media.searchCriteriaTO();
            fcrit[0].column = "categoryId"; fcrit[0].value = managedCatID.ToString();
            fcrit[0].restriction = ScalaWS.Media.restrictionEnum.EQUALS; fcrit[0].restrictionSpecified = true;

            int mediacount = 0;
            mediaTO[] medias = mediaServ.list(fcrit, null);
            foreach (mediaTO media in medias) {
                if (references.Contains(media.id)) {
                    mediacount++;
                    continue;
                }
                Global.d("marked media " + media.id + ": " + media.path + "/" + media.name + " for deletion");
                mediaServ.delete(media.id, true);
            }

            Global.d("garbage collection completed, matched " + msgcount + " messages and " + mediacount + " media items");
            referencedMedia = references;
        }

        public static bool mediaReferenced(int p) {
            if (referencedMedia == null) return true;
            return referencedMedia.Contains(p);
        }
    }

    class ThumbCache {
        Dictionary<int, byte[]> data = new Dictionary<int, byte[]>();
        List<int> lru = new List<int>();
        long totalBytes = 0;
        long maxSize;
        DateTime lastReport = DateTime.MinValue;
        int evicted = 0;
        int replaced = 0;
        public ThumbCache(long maxSize) {
            this.maxSize = maxSize;
        }

        public byte[] this[int id] {
            get {
                return data[id];
            }
            set {
                if (data.ContainsKey(id)) {
                    replaced++;
                    totalBytes -= data[id].Length;
                }
                totalBytes += value.Length;
                data.Add(id, value);
                lru.Remove(id);
                lru.Add(id); // add to end of lru queue
                while (totalBytes > maxSize && data.Count > 0) {
                    // evict least recently used
                    int remove = lru[0]; lru.RemoveAt(0);
                    totalBytes -= data[remove].Length;
                    data.Remove(remove);
                    evicted++;
                }
                if ((DateTime.Now - lastReport).TotalHours >= 1.0) {
                    lastReport = DateTime.Now;

                    Global.d("cache contains " + data.Count + " thumbs totalling " + totalBytes + ", " + replaced + " replaced and " + evicted + " evicted");
                    long check = 0;
                    foreach (int i in data.Keys) {
                        check += data[i].Length;
                    }
                    Global.d(" - crosscheck size = " + check);
                }

            }
        }

        public bool ContainsKey(int id) { return data.ContainsKey(id); }

        public void Remove(int id) {
            if (!data.ContainsKey(id)) return;
            totalBytes -= data[id].Length;
            data.Remove(id);
            lru.Remove(id);
        }
    }

}
