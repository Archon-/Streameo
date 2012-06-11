using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Streameo.Models
{
    public class Search
    {
        string _searchText = "wpisz nazwę artysty lub tytuł piosenki...";
        [RegularExpression(@"^.{3,}$", ErrorMessage = "Musisz podać przynajmniej 3 znaki.")]
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
            }
        }
    }

    public class SearchResults
    {
        public List<Artist> Artists { get; set; }
        public List<Album> Albums { get; set; }
        public List<Song> Songs { get; set; }
    }
}