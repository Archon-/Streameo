using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Streameo.Models
{
    public class Contributions
    {
        int Id { get; set; }
        DateTime Date { get; set; }
        User User { get; set; }
        Song Song { get; set; }
    }
}
