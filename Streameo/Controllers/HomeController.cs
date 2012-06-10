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
                                orderby s.NumberOfPlays descending
                                select s).Take(10).ToList();

            

            List<Album> albums = (from a in db.Albums
                                  select a).ToList();

            List<Artist> artists = (from a in db.Artists
                                    select a).ToList();

            Dictionary<Artist, int> artistPlays = new Dictionary<Artist, int>();
            Dictionary<Album, int> albumPlays = new Dictionary<Album, int>();
            foreach (var artist in artists)
            {
                artistPlays.Add(artist, 0);
                foreach (var album in artist.Albums)
                {
                    albumPlays.Add(album, 0);
                    int plays = 0;
                    foreach (var song in album.Songs)
                    {
                        plays += song.NumberOfPlays;
                    }
                    albumPlays[album] += plays;
                }
                artistPlays[artist] += albumPlays.Last().Value;
            }

            var orderedAlbums = albumPlays.OrderByDescending(kvp => kvp.Value);
            albumPlays = orderedAlbums.Take(10).ToDictionary(k => k.Key, k => k.Value);
            albums = albumPlays.Keys.ToList();

            var orderedArtists = artistPlays.OrderByDescending(kvp => kvp.Value);
            artistPlays = orderedArtists.Take(10).ToDictionary(k => k.Key, k => k.Value);
            artists = artistPlays.Keys.ToList();

            //foreach (var item in songs)
            //{
            //    int newPosition = songs.IndexOf(item);
            //    if (item.Position == -1 && item.PositionImg == null)
            //    {
            //        item.PositionImg = "/Content/images/position-up.png";
            //        item.Position = newPosition;
            //    }
            //    db.Entry<Song>(item).State = System.Data.EntityState.Modified;
            //}

            //foreach (var item in albums)
            //{
            //    int newPosition = albums.IndexOf(item);
            //    if (item.Position == -1 && item.PositionImg == null)
            //    {
            //        item.PositionImg = "/Content/images/position-up.png";
            //        item.Position = newPosition;
            //    }
            //    db.Entry<Album>(item).State = System.Data.EntityState.Modified;
            //}

            //foreach (var item in artists)
            //{
            //    int newPosition = artists.IndexOf(item);
            //    if (item.Position == -1 && item.PositionImg == null)
            //    {
            //        item.PositionImg = "/Content/images/position-up.png";
            //        item.Position = newPosition;
            //    }
            //    db.Entry<Artist>(item).State = System.Data.EntityState.Modified;
            //}

            //db.SaveChanges();

            //if (DateTime.Now.Minute % 2 == 0)
            {
                foreach (var item in songs)
                {
                    int newPosition = songs.IndexOf(item);
                    if(item.Position == -1 || item.Position > newPosition)
                        item.PositionImg = "/Content/images/position-up.png";
                    else if (item.Position == newPosition)
                        item.PositionImg = "/Content/images/position-same.png";
                    else
                        item.PositionImg = "/Content/images/position-down.png";

                    item.Position = newPosition;
                    db.Entry<Song>(item).State = System.Data.EntityState.Modified;
                }

                foreach (var item in albums)
                {
                    int newPosition = albums.IndexOf(item);
                    if (item.Position == -1 || item.Position > newPosition)
                        item.PositionImg = "/Content/images/position-up.png";
                    else if (item.Position == newPosition)
                        item.PositionImg = "/Content/images/position-same.png";
                    else
                        item.PositionImg = "/Content/images/position-down.png";

                    item.Position = newPosition;
                    db.Entry<Album>(item).State = System.Data.EntityState.Modified;
                }

                foreach (var item in artists)
                {
                    int newPosition = artists.IndexOf(item);
                    if (item.Position == -1 || item.Position > newPosition)
                        item.PositionImg = "/Content/images/position-up.png";
                    else if (item.Position == newPosition)
                        item.PositionImg = "/Content/images/position-same.png";
                    else
                        item.PositionImg = "/Content/images/position-down.png";

                    item.Position = newPosition;
                    db.Entry<Artist>(item).State = System.Data.EntityState.Modified;
                }

                db.SaveChanges();
            }

            List<string> covers = new List<string>();
            foreach (var item in songs)
	        {
		         Artist aa = (from a in db.Artists 
                              where a.Name == item.ArtistName 
                              select a).FirstOrDefault();

                covers.Add((from a in aa.Albums
                             where a.Name == item.AlbumName
                             select a.Cover).FirstOrDefault());
	        }

            List<Tuple<Song, string>> songs1 = new List<Tuple<Song, string>>();

            int i = 0;
            foreach (var item in songs)
            {
                songs1.Add(new Tuple<Song,string>(item, covers[i]));
                ++i;
            }

            return View(new Top()
            {
                Songs = songs1,
                Albums = albums,
                Artists = artists
            });
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
