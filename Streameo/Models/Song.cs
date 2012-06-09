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
        public virtual Artist Artist { get; set; }
        public virtual Album Album { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        //public User AddedBy { get; set; }
        public DateTime AddDate { get; set; }
        public int NumberOfPlays { get; set; }
        public virtual List<Voting> Voters { get; set; }
        public double Rating { get; set; }
        public string FilePath { get; set; }
        public int Position { get; set; }
        public string PositionImg { get; set; }
    }

    public class Top
    {
        public List<Song> Songs { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }        
    }

    public class Voting
    {
        public int Id { get; set; }
        public double Vote { get; set; }
        public string User { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        public virtual Artist Artist { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public int Position { get; set; }
        public string PositionImg { get; set; }
        public int NumberOfPlays { get; set; }
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public string PositionImg { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public int NumberOfPlays { get; set; }
        public string Picture { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
