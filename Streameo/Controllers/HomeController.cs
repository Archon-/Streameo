using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streameo.Models;

namespace Streameo.Controllers
{
    public class HomeController : Controller
    {
        DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            List<Song> songs = (from s in db.Songs
                                orderby s.Rating descending
                                select s).ToList();

            List<Song> albums = (from s in songs
                                 select s).Distinct(new DistinctAlbum()).ToList();

            List<Song> artists = (from s in songs
                                  select s).Distinct(new DistinctArtist()).ToList();

            return View(new Top() 
            {
                Songs = songs,
                Albums = albums,
                Artists = artists
            });
        }

        public ActionResult About()
        {
            return View();
        }

        public class DistinctAlbum : IEqualityComparer<Song>
        {
            public bool Equals(Song x, Song y)
            {
                return x.Album.Equals(y.Album);
            }

            public int GetHashCode(Song obj)
            {
                return obj.Album.GetHashCode();
            }
        }

        public class DistinctArtist : IEqualityComparer<Song>
        {
            public bool Equals(Song x, Song y)
            {
                return x.Artist.Equals(y.Artist);
            }

            public int GetHashCode(Song obj)
            {
                return obj.Artist.GetHashCode();
            }
        }
    }
}
