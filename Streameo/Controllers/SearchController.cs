using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streameo.Models;

namespace Streameo.Controllers
{
    public class SearchController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        //
        // GET: /Search/

        [ChildActionOnly]
        public PartialViewResult Index()
        {
            return PartialView(new Search());
        }

        [HttpPost]
        public ActionResult Search(Search form)
        {
            string searchValue = form.SearchText;
            List<Artist> artists = (from a in db.Artists
                                    where a.Name.Contains(searchValue)
                                    select a).ToList();

            List<Album> albums = (from a in db.Albums
                                  where a.Name.Contains(searchValue) //||
                                        //a.ArtistName.Contains(searchValue)
                                  select a).ToList();

            List<Song> songs = (from s in db.Songs
                                  where s.Title.Contains(searchValue) ||
                                        //s.ArtistName.Contains(searchValue) ||
                                        //s.AlbumName.Contains(searchValue) ||
                                        s.Genre.Contains(searchValue)
                                  select s).ToList();

            SearchResults results = new SearchResults() 
            {
                Artists = artists,
                Albums = albums,
                Songs = songs
            };
            return PartialView("Results", results);
        }
    }
}
