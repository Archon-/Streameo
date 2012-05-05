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
                            AddDate = DateTime.Now
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