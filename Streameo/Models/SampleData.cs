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
            //    PremiumEnd = new DateTime(2012, 6, 30),
            //    PremiumStatus = true,
            //    RegistrationDate = DateTime.Now,
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
                foreach (string directory in Directory.GetDirectories(sDir))
                {
                    if (directory.Contains("tmp"))
                        return;

                    foreach (string file in Directory.GetFiles(directory, "*.mp3"))
                    {
                        TagLib.File f = TagLib.File.Create(file);
                        string filePath = file.Replace(HttpContext.Current.Server.MapPath("/Music/"), "");
                        context.Songs.Add(new Song()
                        {
                            Title = f.Tag.Title,
                            Artist = f.Tag.FirstPerformer,
                            Album = f.Tag.Album,
                            Genre = f.Tag.FirstGenre,
                            FilePath = filePath,
                            AddDate = DateTime.Now,
                            Rating = 0,
                            Voters = new List<Voting>(),
                            NumberOfPlays = 0,
                            NumberOfComments = 0
                        });
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