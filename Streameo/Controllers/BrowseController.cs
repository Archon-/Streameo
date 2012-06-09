using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streameo.Models;

namespace Streameo.Controllers
{
    public class BrowseController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Browse/

        public ViewResult Index()
        {
            List<string> genres = (from s in db.Songs
                                   select s.Genre).Distinct().ToList();

            return View(genres);
        }

        //
        // GET: /Browse/Genre/5

        public ViewResult Genre(string name)
        {
            List<Song> songs = (from s in db.Songs
                                select s).ToList();

            List<Song> artists = (from s in songs
                                  where s.Genre == name
                                  orderby s.Genre
                                  select s).Distinct(new Streameo.Controllers.HomeController.DistinctArtist()).ToList();
            return View(artists);
        }

        //
        // GET: /Browse/Artist/5

        public ViewResult Artist(string artist)
        {
            List<Song> songs = (from s in db.Songs
                                select s).ToList();

            List<Song> albums = (from s in songs
                                 where s.Artist == artist
                                 select s).Distinct(new Streameo.Controllers.HomeController.DistinctAlbum()).ToList();
            return View(albums);
        }

        //
        // GET: /Browse/Album/5

        public ViewResult Album(string artist, string album)
        {
            List<Song> album1 = (from s in db.Songs
                                 where s.Album == album &&
                                       s.Artist == artist
                                 select s).ToList();

            foreach (var item in album1)
            {
                if (item.Voters == null)
                {
                    item.Voters = new List<Voting>();
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return View(album1);
        }

        //
        // GET: /Browse/Details/5

        public ViewResult Details(int id)
        {
            Song song = db.Songs.Find(id);
            return View(song);
        }

        [HttpPost]
        public double Rate(int id, double rating)
        {
            if (ModelState.IsValid)
            {
                Song song = db.Songs.Find(id);
                if (song.Voters == null)
                    song.Voters = new List<Voting>();
                song.Voters.Add(new Voting() { Vote = rating, User = User.Identity.Name});

                if (song.Voters != null && song.Voters.Count > 0)
                {
                    double rat = 0;
                    foreach (var item in song.Voters)
                    {
                        rat += item.Vote;
                    }
                    song.Rating = rat / song.Voters.Count;
                }

                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return song.Rating;
            }

            return rating;
        }

        //
        // GET: /Browse/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Browse/Create

        [HttpPost]
        public ActionResult Create(Song song)
        {
            if (ModelState.IsValid)
            {
                db.Songs.Add(song);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(song);
        }

        //
        // GET: /Browse/Edit/5

        public ActionResult Edit(int id)
        {
            Song song = db.Songs.Find(id);
            return View(song);
        }

        //
        // POST: /Browse/Edit/5

        [HttpPost]
        public ActionResult Edit(Song song)
        {
            if (ModelState.IsValid)
            {
                db.Entry(song).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(song);
        }

        //
        // GET: /Browse/Delete/5

        public ActionResult Delete(int id)
        {
            Song song = db.Songs.Find(id);
            return View(song);
        }

        //
        // POST: /Browse/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}