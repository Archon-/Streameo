using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class Payments
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime Date { get; set; }
        User User { get; set; }
    }
}
