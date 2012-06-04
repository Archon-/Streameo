using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Streameo.Models
{
    public static class Premium
    {
        public const double Amount = 40.0;
        public const string Description = "PREMIUM PACKAGE 30 DAYS";

        public static string Control(string name)
        {
            DatabaseContext db = new DatabaseContext();
            var user = (from s in db.Users where s.Email == name select s).FirstOrDefault();
            if (user != null)
                return user.PaymentId;
            else return "test";
        }
    }
}