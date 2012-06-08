using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Streameo.Models;
using NAudio.Wave;
using System.Web.Security;

namespace Streameo.Controllers
{
    public class PlayerController : Controller
    {
        //
        // GET: /Player/
        DatabaseContext db = new DatabaseContext();

        public ActionResult ListenFile(int id)
        {
            List<Song> song = (from s in db.Songs
                                 where s.Id == id
                                 select s).ToList();

            User user = null;

            if(User.Identity.IsAuthenticated)
                 user = (from u in db.Users
                         where u.Email == User.Identity.Name
                         select u).FirstOrDefault();

            string file = "";
            if (user == null || (user != null && !user.IsPremiumAccount()))
            {
                file = Server.MapPath("~/Music/" + song.First().FilePath);
                string tmpFilePath = Server.MapPath("~/Music/tmp/30s/" + song.First().FilePath);
                file = SplitMP3(file, tmpFilePath, 31);
            }
            else if (user != null && user.IsPremiumAccount())
                file = Server.MapPath("~/Music/" + song.First().FilePath);

            return File(file, "audio/mp3");
        }

        public static void CopyStream(Stream input, Stream output, int length)
        {
            // 44100
            byte[] buffer = new byte[32768];
            int len;
            int total = 0;
            input.Position = 0;
            output.Position = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0 && total < length)
            {
                total += len;
                output.Write(buffer, 0, len);
            }
        }

        string SplitMP3(string SourcePath, string DestPath, int length)
        {
            int lastBackslashIndex = DestPath.LastIndexOf("\\");
            string tmpDirPath = DestPath.Remove(lastBackslashIndex);
            DirectoryInfo dir = new DirectoryInfo(tmpDirPath);

            if (!dir.Exists)
                dir.Create();

            using (var reader = new Mp3FileReader(SourcePath))
            {
                try
                {
                    using (FileStream writer = new FileStream(DestPath, FileMode.Open))
                    {
                        return DestPath;
                    }
                }
                catch (FileNotFoundException ex)
                {
                    using (FileStream writer = new FileStream(DestPath, FileMode.Create))
                    {
                        Mp3Frame frame;
                        while ((frame = reader.ReadNextFrame()) != null && (int)reader.CurrentTime.TotalSeconds < length)
                            writer.Write(frame.RawData, 0, frame.RawData.Length);

                        return DestPath;
                    }
                }
                catch (Exception ex)
                { }

                return "";
            }
        }

        public string ListenData(int id)
        {
            var song = (from s in db.Songs
                           where s.Id == id
                          select s).ToList();
            string songData = "";
            if(song.Count > 0)
                songData = song.First().Title + "!TitleArtistSeparator!" + song.First().Artist;
            return songData;
        }

        //
        // GET: /Player/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Player/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Player/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Player/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Player/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Player/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Player/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
