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
        public string Title { get; set; }
        public string Genre { get; set; }
        //public User AddedBy { get; set; }
        public DateTime AddDate { get; set; }
        public int NumberOfPlays { get; set; }
        public int NumberOfComments { get; set; }
        public int Rating { get; set; }
        public string FilePath { get; set; }
    }
}
