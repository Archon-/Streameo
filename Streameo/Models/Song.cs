using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        //public User AddedBy { get; set; }
        public DateTime AddDate { get; set; }
        public int NumberOfPlays { get; set; }
        public int NumberOfComments { get; set; }
        public virtual List<Voting> Voters { get; set; }
        public double Rating { get; set; }
        public string FilePath { get; set; }
    }

    public class Top
    {
        public List<Song> Songs { get; set; }
        public List<Song> Albums { get; set; }
        public List<Song> Artists { get; set; }        
    }

    public class Voting
    {
        public int Id { get; set; }
        public double Vote { get; set; }
        public string User { get; set; }
    }
}
