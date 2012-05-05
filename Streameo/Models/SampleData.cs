using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Streameo.Models
{
    public class SampleData : DropCreateDatabaseAlways<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            context.Songs.Add(new Song()
            {
                Title = "Ultrazapierdalator",
                Artist = "Tesserakt",
                FilePath = "Tesserakt/The Uknown/01 Ultrarozpiedalator.mp3",
                AddDate = DateTime.Now
            });
            context.Songs.Add(new Song() 
            {
                Title = "111",
                Artist = "Tesserakt",
                FilePath = "Tesserakt/The Uknown/03 111.mp3",
                AddDate = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}