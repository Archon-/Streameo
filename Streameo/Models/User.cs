using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class User
    {
        int Id { get; set; }
        string Name { get; set; }
        int SongsAdded { get; set; }
        int TimeOfListening { get; set; }
        bool PremiumStatus { get; set; }
        DateTime RegistrationDate { get; set; }
    }
}
