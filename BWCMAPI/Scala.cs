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
using System.Threading;

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
        static thumbnailService thumbServ = new thumbnailService(Global.cfg("scalaUser"), Global.cfg("scalaPass"), Global.cfg("scalaURL"));

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

                // get templates in master category
                ScalaWS.Template.searchCriteriaTO[] tcrit = new ScalaWS.Template.searchCriteriaTO[1]; tcrit[0] = new ScalaWS.Template.searchCriteriaTO();
                tcrit[0].column = "categoryId"; tcrit[0].value = managedCatID.ToString();
                tcrit[0].restriction = ScalaWS.Template.restrictionEnum.EQUALS; tcrit[0].restrictionSpecified = true;

                templateTO[] temps = templateServ.list(tcrit, null);
                foreach (templateTO templateTO in temps) {
                    try {
                        Template template = new Template(templateTO.id, templateTO.name);
                        templateDataFieldTO[] fields = templateServ.getFiles(template.id, true);
                        foreach (templateDataFieldTO fieldTO in fields) {
                            if (fieldTO.type != templateFieldTypeEnum.IMAGE) continue;
                            Field field = new Field(fieldTO.name);
                            field.templateFieldID = fieldTO.id;
                            template.fields.Add(field);
                        }

                        fields = templateServ.getFields(template.id, true);
                        foreach (templateDataFieldTO fieldTO in fields) {
                            if (fieldTO.name == "SlideTime")
                                template.durationFieldID = fieldTO.id;
                        }
                        template.sortFields();
                        templates.Add(template);
                    } catch (Exception e) {
                        Global.d("Failed to load template data: " + e);
                    }
                }

                // find playlists
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
                            if (item.mediaId == 0) continue; // sub-playlist?

                            messageTO message = messageServ.get(item.mediaId, true);
                            Slide slide = new Slide(item.mediaId, message.name);
                            player.slides.Add(slide);

                            slide.templateID = message.templateId;

                            if (item.useValidRange) {
                                slide.startDate = item.startValidDate.ToShortDateString();
                                if (slide.startDate == "1/1/0001") slide.startDate = null;
                                slide.stopDate = item.endValidDate.ToShortDateString();
                                if (slide.stopDate == "1/1/0001") slide.stopDate = null;
                            }

                            /* // time range is disabled
                            timeScheduleTO[] times = playlistServ.getTimeSchedules(item.id, true);
                            if (times.Length > 0) {
                                slide.startTime = times[0].startTime;
                                if (slide.startTime == "0:00" || slide.startTime == "00:00") slide.startTime = null;
                                slide.stopTime = times[0].endTime;
                                if (slide.stopTime == "24:00") slide.stopTime = null;
                                slide.days = new List<string>();
                                foreach (weekdayEnum day in times[0].days) {
                                    slide.days.Add(day.ToString().Substring(0,3).ToLower());
                                }
                            }
                            */

                            messageDataFieldTO[] fields = messageServ.getFiles(item.mediaId, true);
                            List<string> foundFields = new List<string>();
                            foreach (messageDataFieldTO fieldTO in fields) {
                                Field field = new Field(fieldTO.name);
                                field.mediaID = Convert.ToInt32(fieldTO.value);
                                field.widget = Widget.renderList.Find(Widget.byMediaID(field.mediaID));
                                if (field.widget == null) {
                                    field.widget = new WidgetImage(); // the non-widget widget
                                }

                                slide.fields.Add(field);
                                foundFields.Add(fieldTO.name);
                            }

                            fields = messageServ.getFields(item.mediaId, true);
                            foreach (messageDataFieldTO fieldTO in fields) {
                                if (fieldTO.name == "SlideTime")
                                    try { slide.duration = Convert.ToInt32(fieldTO.value); } catch {}
                            }
                            if (slide.duration == 0)
                                slide.duration = player.info.defaultDuration;

                            // missing fields = WidgetNone
                            Template t = templates.Find(Template.byID(slide.templateID));
                            foreach (Field tf in t.fields) {
                                if (!foundFields.Contains(tf.name)) {
                                    Field nf = new Field(tf.name);
                                    nf.widget = new WidgetNone();
                                    slide.fields.Add(nf);
                                }
                            }

                        } catch (Exception e) {
                            Global.d("Failed to load playlist item in " + player.name + ": " + e);
                        }
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

        public static byte[] largeThumbnail(int id) {
            string uuid = thumbServ.generateThumbnail(id, true, 672, true, 378, true, thumbnailFormat.PNG, true);
            bool done = false;
            int retries = 10;
            while (!done && retries > 0) {
                Thread.Sleep(200); // hack hack hack
                bool dummy;
                thumbServ.isDone(uuid, out done, out dummy);
                retries--;
            }
            string data = thumbServ.getThumbnailAsBytes(uuid);

            byte[] decode = Convert.FromBase64String(data);
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
                List<messageDataFieldTO> fields = new List<messageDataFieldTO>();
                for (int f=0; f < slide.fields.Count; f++) {
                    Field field = slide.fields[f];
                    Field tfield = template.fields.Find(Field.byName(field.name));
                    if (tfield == null) continue; // no such field in template

                    if (field.widget != null && field.widget is WidgetNone)
                        continue; // no field value

                    messageDataFieldTO fieldTO = new messageDataFieldTO();
                    fieldTO.id = tfield.templateFieldID; fieldTO.idSpecified = true; // use template field ID
                    fieldTO.name = field.name;

                    if (field.widget != null && !(field.widget is WidgetImage))
                        field.mediaID = field.widget.upload();

                    if (field.widget is WidgetTwitter && player.info.defaultTwitter == "") {
                        // update default twitter handle
                        WidgetTwitter wt = (WidgetTwitter)field.widget;
                        player.info.defaultTwitter = wt.handles;
                        Global.dataDirty = true;
                    }

                    fieldTO.value = field.mediaID.ToString();
                    fields.Add(fieldTO);
                }

                if (template.durationFieldID > 0) {
                    messageDataFieldTO fieldTO = new messageDataFieldTO();
                    fieldTO.id = template.durationFieldID; fieldTO.name = "SlideTime";
                    fieldTO.value = slide.duration.ToString();
                    fields.Add(fieldTO);
                }
                
                // workaround braindead 1.x API issues in release 10
                message.approvalStatus = ScalaWS.Message.approvalStatusEnum.DRAFT; message.approvalStatusSpecified = true;
                message = messageServ.create(message, fields.ToArray());
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
                try {
                    messageServ.delete(id, true);
                } catch (Exception e) {
                    Global.d("Failed to delete slide " + id + ": " + e.Message);
                }
                thumbCache.Remove(id);
                if (!replaced.Contains(id))
                    summary += " -- deleted slide " + id;
            }

            // reorder messages in playlist
            ScalaWS.Playlist.playlistItemTO[] items = new ScalaWS.Playlist.playlistItemTO[player.slides.Count];
            Dictionary<int, timeScheduleTO> schedules = new Dictionary<int, timeScheduleTO>();
            for (int s=0; s < player.slides.Count; s++) {
                Slide slide = player.slides[s];
                items[s] = new ScalaWS.Playlist.playlistItemTO();
                items[s].mediaId = slide.id; items[s].mediaIdSpecified = true;

                if (slide.startDate != null || slide.stopDate != null) {
                    items[s].useValidRange = true; items[s].useValidRangeSpecified = true;
                    if (slide.startDate != null) {
                        items[s].startValidDate = DateTime.Parse(slide.startDate); items[s].startValidDateSpecified = true;
                    }
                    if (slide.stopDate != null) {
                        items[s].endValidDate = DateTime.Parse(slide.stopDate); items[s].endValidDateSpecified = true;
                    }
                }

                /* // time range is disabled
                if (slide.startTime != null || slide.stopTime != null || (slide.days != null && slide.days.Count < 7)) {
                    if (slide.startTime == null) slide.startTime = "00:00";
                    if (slide.stopTime == null) slide.stopTime = "24:00"; // no, it doesn't make sense
                    timeScheduleTO sched = new timeScheduleTO();
                    sched.startTime = slide.startTime;
                    sched.endTime = slide.stopTime;
                    sched.days = slide.scalaDays();
                    schedules.Add(slide.id, sched);
                }
                */
            }
            playlistServ.deleteAllPlaylistItems(player.id, true);
            foreach (ScalaWS.Playlist.playlistItemTO item in items) {
                try {
                    ScalaWS.Playlist.playlistItemTO playlistItem = playlistServ.addPlaylistItem(player.id, true, item);
                    if (schedules.ContainsKey(item.mediaId)) {
                        playlistServ.addTimeSchedule(playlistItem.id, true, schedules[item.mediaId]);
                    }
                } catch (Exception e) {
                    Global.d("savePlayer: failed to add item " + item.id + " to playlist " + player.name + ": " + e);
                }
            }

            // save changes locally
            players.Remove(old);
            players.Add(player);

            if (old.info != player.info) {
                summary += " -- updated settings";
                Global.dataDirty = true;
            }

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
            try {
                mediaServ.updateApproval(id, true, approval);
            } catch {
                // throws exceptions if media is already approved -- wtf, Scala?
            }
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
                if ((DateTime.Now - media.lastModified).TotalHours < 1.0) {
                    // don't delete stuff that was just uploaded
                    continue;
                }
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

        public static void queryTest() {
            // list all BWCM messages
            ScalaWS.Message.searchCriteriaTO[] mcrit = new ScalaWS.Message.searchCriteriaTO[1]; mcrit[0] = new ScalaWS.Message.searchCriteriaTO();
            mcrit[0].column = "categoryId"; mcrit[0].value = managedCatID.ToString();
            mcrit[0].restriction = ScalaWS.Message.restrictionEnum.EQUALS; mcrit[0].restrictionSpecified = true;
            messageTO[] messages = messageServ.list(mcrit, null);

            Global.d("retrieved " + messages.Length + " messages in one request");

            ScalaWS.Message.listResultCriteriaTO mlcrit = new ScalaWS.Message.listResultCriteriaTO();
            mlcrit.firstResult = 0; mlcrit.firstResultSpecified = true;
            const int chunk = 20;
            mlcrit.maxResults = chunk; mlcrit.maxResultsSpecified = true;
            List<messageTO> allmessages = new List<messageTO>();
            messages = new messageTO[0];
            do {
                messages = messageServ.list(mcrit, mlcrit);
                Global.d("got chunk with " + messages.Length + " messages");
                mlcrit.firstResult += messages.Length;
                allmessages.Concat<messageTO>(messages);
            } while (messages.Length == chunk);
            Global.d("total " + allmessages.Count + " messages received");
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
