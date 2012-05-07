using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Streameo.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DatabaseContext") { }

        public DbSet<Song> Songs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}