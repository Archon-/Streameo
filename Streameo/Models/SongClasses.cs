using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class Song
    {
        int Id { get; set; }
        string Author { get; set; }
        string Name { get; set; }
        string Genre { get; set; }
        User AddedBy { get; set; }
        DateTime AddDate { get; set; }
        int NumberOfPlays { get; set; }
        int NumberOfComments { get; set; }
    }
}
