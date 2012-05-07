using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public int? SongsAdded { get; set; }
        public int? TimeOfListening { get; set; }
        public bool? PremiumStatus { get; set; }
        public string PaymentId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
