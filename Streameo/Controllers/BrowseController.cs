﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streameo.Models;
using System.IO;

namespace Streameo.Controllers
{
    public class BrowseController : Controller
    {
        private DatabaseContext db = new DatabaseContext();


        /////////////////////////////////////////////////////////////////
        ////////                                                 ////////
        ////////                     GENRES                      ////////
        ////////                                                 ////////
        /////////////////////////////////////////////////////////////////

        //
        // GET: /Browse/

        public ViewResult Index()
        {
            List<string> genres = (from s in db.Songs
                                   select s.Genre).Distinct().ToList();

            List<Artist> artists = (from a in db.Artists
                                    where a.Albums.Count == 0
                                    select a).ToList();

            return View(new Tuple<List<string>, List<Artist>>(genres, artists));
        }

        //
        // GET: /Browse/Genre/5

        public ViewResult Genre(string name)
        {
            List<Artist> artists = (from a in db.Artists
                                    where a.Albums.Count > 0
                                    select a).ToList();

            ViewBag.Genre = name;
            bool nextArtist = false;
            for (int i = 0; i < artists.Count; )
            {
                for (int j = 0; j < artists[i].Albums.Count; ++j)
                {
                    for (int k = 0; k < artists[i].Albums[j].Songs.Count; ++k)
                    {
                        if (artists[i].Albums[j].Songs[k].Genre == name)
                        {
                            nextArtist = true;
                            break;
                        }
                    }
                    if (nextArtist)
                        break;
                }
                if (!nextArtist)
                    artists.RemoveAt(i);
                else
                {
                    nextArtist = false;
                    ++i;
                }
            }

            return View(artists);
        }



        /////////////////////////////////////////////////////////////////
        ////////                                                 ////////
        ////////                     ARTISTS                     ////////
        ////////                                                 ////////
        /////////////////////////////////////////////////////////////////


        //
        // GET: /Browse/Artist/5

        public ViewResult Artist(string artist)
        {
            Artist artist1 = (from a in db.Artists
                              where a.Name == artist
                              select a).FirstOrDefault();
            return View(artist1);
        }

        //public string ArtistName(int id)
        //{
        //    var song = (from s in db.Songs
        //                where s.Id == id
        //                select s).ToList();
        //    string songData = "";
        //    if (song.Count > 0)
        //        songData = song.First().Title + "!TitleArtistSeparator!" + song.First().ArtistName;
        //    return songData;
        //}

        //
        // GET: /Browse/CreateArtist
        [Authorize(Roles = "Admin")]
        public ActionResult CreateArtist()
        {
            return View();
        }

        //
        // POST: /Browse/CreateArtist
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateArtist(Artist artist, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Content/images/artists/");
                    if (!Directory.Exists(path1 + artist.Name))
                        Directory.CreateDirectory(path1 + artist.Name);
                    var path = Server.MapPath("/Content/images/artists/" + artist.Name + "/" + fileName);
                    file.SaveAs(path);

                    artist.Picture = "/Content/images/artists/" + artist.Name + "/" + fileName;
                }
                else
                    artist.Picture = "/Content/images/artists/unknown_artist.png";

                artist.Position = -1;
                artist.Albums = new List<Album>();


                db.Artists.Add(artist);
                db.SaveChanges();
                return RedirectToAction("Artist", new { artist = artist.Name });
            }

            return View(artist);
        }

        //
        // GET: /Browse/EditArtist/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditArtist(int id)
        {
            Artist artist = db.Artists.Find(id);
            return View(artist);
        }

        //
        // POST: /Browse/EditArtist/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditArtist(Artist artist, HttpPostedFileBase file)
        {
            Artist a = artist;
            if (artist != null)
            {
                a = db.Artists.Find(artist.Id);
                a.Name = artist.Name;
            }

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Content/images/artists/");
                    if (!Directory.Exists(path1 + artist.Name))
                        Directory.CreateDirectory(path1 + artist.Name);
                    var path = Server.MapPath("/Content/images/artists/" + artist.Name + "/" + fileName);
                    file.SaveAs(path);

                    a.Picture = "/Content/images/artists/" + artist.Name + "/" + fileName;
                }

                foreach (var item in a.Albums)
                {
                    foreach (var item1 in item.Songs)
                    {
                        item1.ArtistName = a.Name;
                    }
                    item.ArtistName = a.Name;
                }
                db.Entry(a).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Artist", new { artist = a.Name });
            }
            return View(a);
        }

        //
        // GET: /Browse/DeleteArtist/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteArtist(int id)
        {
            Artist artist = db.Artists.Find(id);
            return View(artist);
        }

        //
        // POST: /Browse/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteArtist")]
        public ActionResult DeleteArtistConfirmed(int id)
        {
            Artist artist = db.Artists.Find(id);

            for (int i = 0; i < artist.Albums.Count; )
            {
                for (int j = 0; j < artist.Albums[i].Songs.Count; )
                {
                    db.Songs.Remove(artist.Albums[i].Songs[j]);
                }
                db.Albums.Remove(artist.Albums[i]);
            }

            db.Artists.Remove(artist);


            db.SaveChanges();
            return RedirectToAction("Index");
        }



        /////////////////////////////////////////////////////////////////
        ////////                                                 ////////
        ////////                     ALBUMS                      ////////
        ////////                                                 ////////
        /////////////////////////////////////////////////////////////////


        //
        // GET: /Browse/Album/5

        public ViewResult Album(int id)
        {
            Album album = db.Albums.Find(id);
            return View(album);
        }

        //
        // GET: /Browse/CreateAlbum
        [Authorize(Roles = "Admin")]
        public ActionResult CreateAlbum(string artist)
        {
            Artist art = (from s in db.Artists
                          where s.Name == artist
                          select s).FirstOrDefault();

            var model = new AlbumViewModel
            {
                Artists = db.Artists.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (art != null)
            {
                model.SelectedArtistId = art.Id;
            }

            return View(model);
        }

        //
        // POST: /Browse/CreateArtist
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateAlbum(AlbumViewModel collection, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                Artist artist = db.Artists.Find(collection.SelectedArtistId);
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Content/images/covers/");

                    if (!Directory.Exists(path1 + artist.Name))
                        Directory.CreateDirectory(path1 + artist.Name);

                    if (!Directory.Exists(path1 + artist.Name + "/" + collection.Album.Name))
                        Directory.CreateDirectory(path1 + artist.Name + "/" + collection.Album.Name);

                    var path = Server.MapPath("/Content/images/covers/" + artist.Name + "/" + collection.Album.Name + "/" + fileName);
                    file.SaveAs(path);

                    collection.Album.Cover = "/Content/images/covers/" + artist.Name + "/" + collection.Album.Name + "/" + fileName;
                }
                else
                    collection.Album.Cover = "/Content/images/covers/unknown_album.png";

                collection.Album.Position = -1;
                collection.Album.Songs = new List<Song>();

                if (string.IsNullOrEmpty(collection.Album.Cover))
                    collection.Album.Cover = "/Content/images/covers/unknown_album.png";

                collection.Album.ArtistName = artist.Name;

                artist.Albums.Add(collection.Album);
                db.Entry(artist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Artist", new { artist = artist.Name });
            }

            collection.Artists = db.Artists.ToList().Select(x => new SelectListItem
                                {
                                    Text = x.Name,
                                    Value = x.Id.ToString()
                                });
            return View(collection);
        }


        //
        // GET: /Browse/EditAlbum/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditAlbum(int id)
        {
            var model = new AlbumViewModel();
            model.Album = db.Albums.Find(id);
            Artist art = (from s in db.Artists
                          where s.Name == model.Album.ArtistName
                          select s).FirstOrDefault();

            model.Artists = db.Artists.ToList().Select(x => new SelectListItem
                            {
                                Text = x.Name,
                                Value = x.Id.ToString()
                            });

            if (art != null)
            {
                model.SelectedArtistId = art.Id;
            }

            return View(model);
        }

        //
        // POST: /Browse/EditAlbum/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditAlbum(AlbumViewModel collection, HttpPostedFileBase file)
        {
            Album a2 = collection.Album;
            if (collection.Album != null)
            {
                a2 = db.Albums.Find(collection.Album.Id);
                a2.Name = collection.Album.Name;
            }

            if (ModelState.IsValid)
            {
                Artist newArtist = db.Artists.Find(collection.SelectedArtistId);
                Album a = db.Albums.Find(collection.Album.Id);

                if (a.ArtistName != newArtist.Name)
                {
                    Artist oldArtist = (from a1 in db.Artists
                                        where a1.Name == a.ArtistName
                                        select a1).FirstOrDefault();

                    oldArtist.Albums.Remove(a);
                    foreach (var item1 in a.Songs)
                    {
                        item1.ArtistName = a.ArtistName;
                    }
                    a.Name = collection.Album.Name;
                    a.ArtistName = newArtist.Name;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string path1 = Server.MapPath("/Content/images/covers/");

                        if (!Directory.Exists(path1 + a.ArtistName))
                            Directory.CreateDirectory(path1 + a.ArtistName);

                        if (!Directory.Exists(path1 + a.ArtistName + "/" + a.Name))
                            Directory.CreateDirectory(path1 + a.ArtistName + "/" + a.Name);

                        var path = Server.MapPath("/Content/images/covers/" + a.ArtistName + "/" + a.Name + "/" + fileName);
                        file.SaveAs(path);

                        a.Cover = "/Content/images/covers/" + a.ArtistName + "/" + a.Name + "/" + fileName;
                    }

                    newArtist.Albums.Add(a);

                    db.Entry(oldArtist).State = EntityState.Modified;
                    db.Entry(newArtist).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Artist", new { artist = newArtist.Name });
                }

                foreach (var item1 in a.Songs)
                {
                    item1.ArtistName = a.ArtistName;
                }
                a.Name = collection.Album.Name;
                a.ArtistName = newArtist.Name;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Content/images/covers/");

                    if (!Directory.Exists(path1 + a.ArtistName))
                        Directory.CreateDirectory(path1 + a.ArtistName);

                    if (!Directory.Exists(path1 + a.ArtistName + "/" + a.Name))
                        Directory.CreateDirectory(path1 + a.ArtistName + "/" + a.Name);

                    var path = Server.MapPath("/Content/images/covers/" + a.ArtistName + "/" + a.Name + "/" + fileName);
                    file.SaveAs(path);

                    a.Cover = "/Content/images/covers/" + a.ArtistName + "/" + a.Name + "/" + fileName;
                }

                db.Entry(a).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Artist", new { artist = newArtist.Name });
            }
            collection.Album = a2;
            collection.Artists = db.Artists.ToList().Select(x => new SelectListItem
                                {
                                    Text = x.Name,
                                    Value = x.Id.ToString()
                                });
            return View(collection);
        }

        //
        // GET: /Browse/DeleteAlbum/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAlbum(int id)
        {
            Album album = db.Albums.Find(id);
            return View(album);
        }

        //
        // POST: /Browse/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteAlbum")]
        public ActionResult DeleteAlbumConfirmed(int id)
        {
            Album album = db.Albums.Find(id);

            for (int j = 0; j < album.Songs.Count; )
            {
                db.Songs.Remove(album.Songs[j]);
            }

            db.Albums.Remove(album);

            db.SaveChanges();
            return RedirectToAction("Index");
        }




        /////////////////////////////////////////////////////////////////
        ////////                                                 ////////
        ////////                     SONGS                       ////////
        ////////                                                 ////////
        /////////////////////////////////////////////////////////////////


        //
        // GET: /Browse/CreateSong
        [Authorize(Roles = "Admin")]
        public ActionResult CreateSong(string artist, string album)
        {
            var model = new SongViewModel
            {
                Artists = db.Artists.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };


            Artist art = (from s in db.Artists
                          where s.Name == artist
                          select s).FirstOrDefault();

            if (art == null)
                art = (from s in db.Artists
                       select s).FirstOrDefault();

            if (art != null)
            {
                model.SelectedArtistId = art.Id;
                model.Albums = art.Albums.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                Album alb = (from s in db.Albums
                             where s.Name == album
                             select s).FirstOrDefault();

                if (alb != null)
                {
                    model.SelectedAlbumId = alb.Id;

                    if (alb.Songs.Count > 0)
                    {
                        model.Song = new Song();
                        model.Song.Genre = alb.Songs[0].Genre;
                    }
                }
            }

            return View(model);
        }

        public ActionResult AlbumList(int Id)
        {
            Artist artist = db.Artists.Find(Id);
            List<Album> Albums = (from a in db.Albums
                                  where a.ArtistName == artist.Name
                                  select a).ToList();

            var albums1 = Albums.Select(x => new
            {
                Id = x.Id,
                Name = x.Name
            });
            return Json(albums1, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Browse/CreateSong
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateSong(SongViewModel collection, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                Song s = new Song();
                s.AddDate = DateTime.Now;
                Album album = db.Albums.Find(collection.SelectedAlbumId);
                s.AlbumName = album.Name;
                Artist artist = db.Artists.Find(collection.SelectedArtistId);
                s.ArtistName = artist.Name;
                s.Genre = collection.Song.Genre;
                s.NumberOfPlays = 0;
                s.Position = -1;
                s.Rating = 0;
                s.Title = collection.Song.Title;
                s.Voters = new List<Voting>();

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Music/");

                    if (!Directory.Exists(path1 + artist.Name))
                        Directory.CreateDirectory(path1 + artist.Name);

                    if (!Directory.Exists(path1 + artist.Name + "/" + album.Name))
                        Directory.CreateDirectory(path1 + artist.Name + "/" + album.Name);

                    var path = Server.MapPath("/Music/" + artist.Name + "/" + album.Name + "/" + fileName);
                    file.SaveAs(path);

                    s.FilePath = artist.Name + "\\" + album.Name + "\\" + fileName;
                }

                album.Songs.Add(s);
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Album", new { id = album.Id });
            }


            collection.Artists = db.Artists.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            Artist art = (from s in db.Artists
                          where s.Id == collection.SelectedArtistId
                          select s).FirstOrDefault();
            if (art != null)
            {
                collection.Albums = art.Albums.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            return View(collection);
        }

        //
        // POST: /Browse/CreateSongFromTags
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateSongFromTags(HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string cover = "/Content/images/covers/unknown_album.png";
                    string picture = "/Content/images/artists/unknown_artist.png";

                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Music/");

                    file.SaveAs(path1 + "/file1.mp3");

                    TagLib.File f = TagLib.File.Create(path1 + "/file1.mp3");
                    string albumName = f.Tag.Album;
                    string artistName = f.Tag.FirstPerformer;

                    if (!Directory.Exists(path1 + artistName))
                        Directory.CreateDirectory(path1 + artistName);

                    if (!Directory.Exists(path1 + artistName + "/" + albumName))
                        Directory.CreateDirectory(path1 + artistName + "/" + albumName);

                    var path = Server.MapPath("/Music/" + artistName + "/" + albumName + "/" + fileName);
                    file.SaveAs(path);

                    string filePath = artistName + "\\" + albumName + "\\" + fileName;

                    Song s = new Song()
                    {
                        Title = f.Tag.Title,
                        ArtistName = artistName,
                        AlbumName = albumName,
                        Genre = f.Tag.FirstGenre,
                        FilePath = filePath,
                        AddDate = DateTime.Now,
                        Rating = 0,
                        Voters = new List<Voting>(),
                        NumberOfPlays = 0,
                        Position = -1
                    };
                    List<Song> Songs = new List<Song>();
                    Songs.Add(s);

                    Album Album = new Album()
                        {
                            Name = albumName,
                            ArtistName = artistName,
                            Position = -1,
                            PositionImg = "",
                            Cover = cover,
                            Songs = Songs
                        };
                    List<Album> Albums = new List<Models.Album>();
                    Albums.Add(Album);

                    Artist artist = (from a in db.Artists
                                     where a.Name == artistName
                                     select a).FirstOrDefault();

                    if (artist == null)
                    {
                        artist = new Artist()
                        {
                            Name = artistName,
                            Position = -1,
                            PositionImg = "",
                            Picture = picture,
                            Albums = Albums
                        };
                        db.Artists.Add(artist);
                    }
                    else
                    {
                        Album aa = (from a in artist.Albums
                                    where a.Name == albumName
                                    select a).FirstOrDefault();

                        if (aa == null)
                        {
                            artist.Albums.Add(Album);
                            db.Entry(artist).State = System.Data.EntityState.Modified;
                        }
                        else
                        {
                            aa.Songs.Add(s);
                            db.Entry(aa).State = System.Data.EntityState.Modified;
                        }
                    }

                    db.SaveChanges();

                    Album aaa = (from a in db.Albums
                                 where a.Name == albumName
                                 select a).FirstOrDefault();

                    return RedirectToAction("Album", new { id = aaa.Id });
                }

            }
        

            return View();
        }


        //
        // GET: /Browse/EditSong/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditSong(int id, string artist, string album)
        {
            Artist art = (from s in db.Artists
                          where s.Name == artist
                          select s).FirstOrDefault();

            var model = new SongViewModel
            {
                Artists = db.Artists.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            if (art != null)
            {
                model.SelectedArtistId = art.Id;
                model.Albums = art.Albums.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                Album alb = (from s in db.Albums
                             where s.Name == album
                             select s).FirstOrDefault();

                if (alb != null)
                {
                    model.SelectedAlbumId = alb.Id;
                }
            }

            model.Song = db.Songs.Find(id);
            return View(model);
        }

        //
        // POST: /Browse/EditSong/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditSong(SongViewModel collection, HttpPostedFileBase file)
        {
            Song s2 = collection.Song;
            if (collection.Song != null)
            {
                s2 = db.Songs.Find(collection.Song.Id);
                s2.Title = collection.Song.Title;
            }

            if (ModelState.IsValid)
            {
                Song s = db.Songs.Find(collection.Song.Id);

                Album newAlbum = db.Albums.Find(collection.SelectedAlbumId);
                Artist newArtist = db.Artists.Find(collection.SelectedArtistId);

                if (s.AlbumName != newAlbum.Name || s.ArtistName != newArtist.Name)
                {
                    Album oldAlbum = (from a in db.Albums
                                      where a.Name == s.AlbumName
                                      select a).FirstOrDefault();

                    oldAlbum.Songs.Remove(s);
                    s.Genre = collection.Song.Genre;
                    s.Title = collection.Song.Title;
                    s.AlbumName = newAlbum.Name;
                    s.ArtistName = newArtist.Name;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        string path1 = Server.MapPath("/Music/");

                        if (!Directory.Exists(path1 + s.ArtistName))
                            Directory.CreateDirectory(path1 + s.ArtistName);

                        if (!Directory.Exists(path1 + s.ArtistName + "/" + s.AlbumName))
                            Directory.CreateDirectory(path1 + s.ArtistName + "/" + s.AlbumName);

                        var path = Server.MapPath("/Music/" + s.ArtistName + "/" + s.AlbumName + "/" + fileName);
                        file.SaveAs(path);

                        s.FilePath = s.ArtistName + "\\" + s.AlbumName + "\\" + fileName;
                    }

                    newAlbum.Songs.Add(s);

                    db.Entry(oldAlbum).State = EntityState.Modified;
                    db.Entry(newAlbum).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Album", new { id = newAlbum.Id });
                }

                s.Genre = collection.Song.Genre;
                s.Title = collection.Song.Title;
                s.AlbumName = newAlbum.Name;
                s.ArtistName = newArtist.Name;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string path1 = Server.MapPath("/Music/");

                    if (!Directory.Exists(path1 + s.ArtistName))
                        Directory.CreateDirectory(path1 + s.ArtistName);

                    if (!Directory.Exists(path1 + s.ArtistName + "/" + s.AlbumName))
                        Directory.CreateDirectory(path1 + s.ArtistName + "/" + s.AlbumName);

                    var path = Server.MapPath("/Music/" + s.ArtistName + "/" + s.AlbumName + "/" + fileName);
                    file.SaveAs(path);

                    s.FilePath = s.ArtistName + "\\" + s.AlbumName + "\\" + fileName;
                }

                db.Entry(s).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Album", new { id = newAlbum.Id });
            }

            collection.Song = s2;
            collection.Artists = db.Artists.ToList().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            Artist art = (from s in db.Artists
                          where s.Id == collection.SelectedArtistId
                          select s).FirstOrDefault();
            if (art != null)
            {
                collection.Albums = art.Albums.ToList().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }

            return View(collection);
        }

        //
        // GET: /Browse/DeleteSong/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteSong(int id)
        {
            Song song = db.Songs.Find(id);
            return View(song);
        }

        //
        // POST: /Browse/DeleteSong/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteSong")]
        public ActionResult DeleteSongConfirmed(int id)
        {
            Song song = db.Songs.Find(id);
            Album album = (from a in db.Albums
                           where a.Name == song.AlbumName
                           select a).FirstOrDefault();

            db.Songs.Remove(song);

            db.SaveChanges();
            return RedirectToAction("Album", new { id = album.Id });
        }



        [HttpPost]
        public double Rate(int id, double rating)
        {
            if (ModelState.IsValid)
            {
                Song song = db.Songs.Find(id);
                if (song.Voters == null)
                    song.Voters = new List<Voting>();
                song.Voters.Add(new Voting() { Vote = rating, User = User.Identity.Name });

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}