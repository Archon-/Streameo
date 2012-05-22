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
        public DateTime? PremiumEnd { get; set; }

        public bool IsPremiumAccount()
        {
            if (PremiumEnd == null)
                return false;
            else if (PremiumEnd > DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public void AddDays(int days)
        {
            if (PremiumEnd == null)
            {
                PremiumEnd = DateTime.Now;
                PremiumEnd += new TimeSpan(days, 0, 0, 0);
            }
            else if (PremiumEnd > DateTime.Now)
            {
                PremiumEnd = DateTime.Now + new TimeSpan(days, 0, 0, 0);
            }
            else
            {
                PremiumEnd += new TimeSpan(days, 0, 0, 0);
            }
        }

        public void DeleteDays(int days)
        {
            PremiumEnd = DateTime.Now - new TimeSpan(days, 0, 0, 0);
        }
    }
}
