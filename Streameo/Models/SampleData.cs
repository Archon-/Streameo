using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.IO;

namespace Streameo.Models
{
    public class SampleData : DropCreateDatabaseAlways<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            AddAllMusic(context, HttpContext.Current.Server.MapPath("/Music/"));

            //context.Users.Add(new User()
            //{
            //    Id = 0,
            //    Email = "o@p.l",
            //    PaymentId = "0",
            //    PremiumEnd = new DateTime(2012, 2, 30),
            //    PremiumStatus = false,
            //    RegistrationDate = new DateTime(2012, 1, 30),
            //    SongsAdded = 0,
            //    TimeOfListening = 0
            //});

            //context.Users.Add(new User()
            //{
            //    Id = 1,
            //    Email = "o1@p.l",
            //    PaymentId = "1",
            //    PremiumEnd = new DateTime(2012, 5, 30),
            //    PremiumStatus = false,
            //    RegistrationDate = new DateTime(2012, 5, 1),
            //    SongsAdded = 0,
            //    TimeOfListening = 0
            //});


            context.SaveChanges();
        }

        void AddAllMusic(DatabaseContext context, string sDir)
        {
            try
            {
                Artist artist = null;
                Album Album = new Album();
                List<Song> Songs = new List<Song>();
                string artistName = "";
                foreach (string directory in Directory.GetDirectories(sDir))
                {
                    if (directory.Contains("tmp"))
                        return;

                    Songs = new List<Song>();
                    Album = null;
                    string albumName = "";
                    //string[] images = Directory.GetFiles(directory, "*.jpg");
                    string cover = "/Content/images/covers/unknown_album.png";
                    string picture = "/Content/images/artists/unknown_artist.png";
                    //if (images.Length > 0)
                    //{
                    //    cover = "/Content/images/covers/" + images[0].Replace(HttpContext.Current.Server.MapPath("/Music/"), "");
                    //    cover = cover.Replace("\\", "/");
                    //}
                    foreach (string file in Directory.GetFiles(directory, "*.mp3"))
                    {
                        TagLib.File f = TagLib.File.Create(file);
                        albumName = f.Tag.Album;
                        artistName = f.Tag.FirstPerformer;
                        string filePath = file.Replace(HttpContext.Current.Server.MapPath("/Music/"), "");

                        if (cover == "/Content/images/covers/unknown_album.png")
                        {
                            int indexof = filePath.LastIndexOf("\\");
                            string filePath1 = filePath.Substring(0, indexof);

                            try
                            {
                                foreach (string img in Directory.GetFiles(HttpContext.Current.Server.MapPath("/Content/images/covers/" + filePath1), "*.jpg"))
                                {
                                    cover = img.Replace(HttpContext.Current.Server.MapPath("/"), "");
                                    cover = cover.Replace("\\", "/");
                                    if (!cover.StartsWith("/"))
                                        cover = cover.Insert(0, "/");
                                    break;
                                }
                            }
                            catch (Exception ex) { }
                        }

                        if (picture == "/Content/images/artists/unknown_artist.png")
                        {
                            string tmp = filePath;
                            if (filePath.StartsWith("\\"))
                                tmp = filePath.Remove(0, 2);

                            int indexof = tmp.IndexOf("\\");
                            string filePath1 = tmp.Substring(0, indexof);

                            try
                            {
                                foreach (string img in Directory.GetFiles(HttpContext.Current.Server.MapPath("/Content/images/artists/" + filePath1), "*.jpg"))
                                {
                                    picture = img.Replace(HttpContext.Current.Server.MapPath("/"), "");
                                    picture = picture.Replace("\\", "/");
                                    if (!picture.StartsWith("/"))
                                        picture = picture.Insert(0, "/");
                                    break;
                                }
                            }
                            catch (Exception ex) { }
                        }

                        //if (artist == null || (artist != null && artist.Name != f.Tag.FirstPerformer))
                        //    artist = new Artist()
                        //    {
                        //        Name = f.Tag.FirstPerformer,
                        //        Position = -1,
                        //        PositionImg = "",
                        //        Comments = new List<Comment>(),
                        //        NumberOfPlays = 0,
                        //        Picture = picture,
                        //        Albums = new List<Album>()
                        //    };

                        //if (album == null || (album != null && album.Name != f.Tag.Album))
                        //    album = new Album()
                        //    {
                        //        Name = f.Tag.Album,
                        //        Position = -1,
                        //        PositionImg = "",
                        //        Cover = cover,
                        //        NumberOfPlays = 0,
                        //        Artist = artist
                        //    };
                        Songs.Add(new Song()
                        {
                            Title = f.Tag.Title,
                            ArtistName = artistName,
                            AlbumName = albumName,
                            //Artist = artist,
                            //Album = album,
                            Genre = f.Tag.FirstGenre,
                            FilePath = filePath,
                            AddDate = DateTime.Now,
                            Rating = 0,
                            Voters = new List<Voting>(),
                            NumberOfPlays = 0,
                            Position = -1
                        });
                        //context.Songs.Add(Songs.Last());
                    }

                    if (Songs.Count > 0)
                    {
                        Album  = new Album()
                        {
                            Name = albumName,
                            ArtistName = artistName,
                            Position = -1,
                            PositionImg = "",
                            Cover = cover,
                            Songs = Songs
                            //NumberOfPlays = 0
                        };

                        if (artist == null || (artist != null && artist.Name != artistName))
                        {
                            artist = new Artist()
                            {
                                Name = artistName,
                                Position = -1,
                                PositionImg = "",
                                Comments = new List<Comment>(),
                                //NumberOfPlays = 0,
                                Picture = picture,
                                Albums = new List<Album>()
                            };
                            context.Artists.Add(artist);
                            context.SaveChanges();
                        }

                        artist.Albums.Add(Album);
                        context.Entry(artist).State = System.Data.EntityState.Modified;
                        context.SaveChanges();
                    }


                    AddAllMusic(context, directory);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}