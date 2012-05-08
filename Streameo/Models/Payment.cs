using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Streameo.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual Models.User User { get; set; }
        public double Amount { get; set; }
        public string T_id { get; set; }
        public DateTime? DateTime { get; set; }
        public int Status { get; set; }
    }
}