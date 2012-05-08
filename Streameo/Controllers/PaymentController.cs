using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Streameo.Models;
using System.Globalization;

namespace Streameo.Controllers
{
    public class PaymentController : Controller
    {
        //
        // GET: /Payment/

        public ActionResult Index()
        {
            return View();
        }

        public string URLC(int id, string control, string t_id, string amount, [Bind(Include = "e-mail")]string email, string t_status, string md5)
        {
            string textToBeHashed = String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}",
                                              Dotpay.PIN,
                                              id,
                                              control == null ? "" : control,
                                              t_id == null ? "" : t_id,
                                              amount == null ? "" : amount,
                                              email == null ? "" : email,
                                              "",
                                              "",
                                              "",
                                              "",
                                              t_status == null ? "" : t_status);

            if (Dotpay.IsValidNotyfication(textToBeHashed, md5) && double.Parse(amount, CultureInfo.InvariantCulture) == Premium.Amount) // TODO: IP
            {
                DatabaseContext db = new DatabaseContext();
                List<Models.Payment> lp = (from s in db.Payments where s.T_id == t_id select s).ToList();

                if (lp.Count == 1)
                {
                    lp[0].Status = int.Parse(t_status);
                    int UserID = lp[0].UserID;
                    Models.User us = (from s in db.Users where s.Id == UserID select s).FirstOrDefault();
                    
                    if (lp[0].Status == 2)
                        us.AddDays(30);
                    else if (lp[0].Status == 4 || lp[0].Status == 5)
                        us.DeleteDays(30);
                }
                else
                {
                    List<Models.User> us = (from s in db.Users where s.PaymentId == control select s).ToList();
                    if (us.Count == 0)
                        return "";

                    db.Payments.Add(new Models.Payment() { T_id = t_id, Amount = double.Parse(amount, CultureInfo.InvariantCulture), Status = int.Parse(t_status), UserID = us[0].Id });
                }
                db.SaveChanges();
                return "OK";

            }
            else
            {
                return "";
            }
        }
    }
}
