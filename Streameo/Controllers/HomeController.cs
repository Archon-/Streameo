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
                                  orderby a.NumberOfPlays descending
                                  select a).Take(10).ToList();

            List<Artist> artists = (from a in db.Artists
                                    orderby a.NumberOfPlays descending
                                    select a).Take(10).ToList();

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
    }
}
