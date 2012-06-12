using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Streameo.Models;
using System.Web.Helpers;
using Facebook;
using System.IO;
using System.Text;
using System.Net;

namespace Streameo.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Nazwa użytkownika lub hasło nieprawidłowe.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Email, model.Password, model.Email, null, null, false, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser account = Membership.GetUser(model.Email);

                    User user = new User();
                    user.Email = account.Email;
                    user.RegistrationDate = account.CreationDate;
                    user.PaymentId = Guid.NewGuid().ToString("N");
                    user.ActivationKey = Guid.NewGuid().ToString("N");

                    if (ModelState.IsValid)
                    {
                        DatabaseContext db = new DatabaseContext();

                        db.Users.Add(user);
                        db.SaveChanges();

                        Roles.AddUserToRole(account.Email, "User");

                        WebMail.SmtpServer = "smtp.gmail.com";
                        WebMail.EnableSsl = true;
                        WebMail.SmtpPort = 587;
                        WebMail.UserName = "test.owalski@gmail.com";
                        WebMail.Password = "haslotestowe";
                        WebMail.Send(
                                account.Email,
                                "Aktywacja konta na Streameo",
                                "Witaj!<br /><br />" +
                                "Kliknij w poniższy link aby aktywować konto.<br /><br />" +
                                "<a href=\"" + Url.Action("Activate", "Account", new { key = user.ActivationKey }, Request.Url.Scheme) + "\">Aktywacja</a>"
                            );
                    }
                    else
                    {
                        Membership.DeleteUser(account.Email);
                    }
                    
                    //FormsAuthentication.SetAuthCookie(model.Email, false /* createPersistentCookie */);
                    return RedirectToAction("RegisterSuccess", "Account");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("OldPassword", "Aktualne hasło jest nieprawidłowe.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult Activate(string key)
        {
            DatabaseContext db = new DatabaseContext();

            var result = from u in db.Users
                    where u.ActivationKey == key// && u.Pass == pass
                    select u;

            if (result.Count() == 1)
            {
                User user = result.First();

                MembershipUser account = Membership.GetUser(user.Email);
                account.IsApproved = true;
                Membership.UpdateUser(account);

                FormsAuthentication.SetAuthCookie(account.Email, false /* createPersistentCookie */);

                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        public ActionResult Facebook()
        {
            return new RedirectResult("https://graph.facebook.com/oauth/authorize?type=web_server&client_id=144618862327767&redirect_uri=http://localhost:1188/account/handshake/&scope=email%2Coffline_access%2Cuser_about_me");
        }

        public ActionResult Handshake(string code)
        {
            bool flag = true;
            string clientId = "144618862327767";
            string clientSecret = "48af78235494ff833ed27d91d89a903d";

            //musimy wyslac zadanie w celu otrzymania access tokena
            string url = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

            string redirectUri = "http://localhost:1188/account/handshake/";

             WebRequest request = WebRequest.Create(string.Format(url, clientId, redirectUri, clientSecret, code));

            //przekonwertuj odpowiedz do utf8 i wyciagnij access tokena
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(stream, encode);
            string accessToken = streamReader.ReadToEnd().Replace("access_token=", "");
            streamReader.Close();
            response.Close();

            var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me");

            string email = me.email;
            string password = Membership.GeneratePassword(20, 6);

            DatabaseContext db = new DatabaseContext();

            var result = from u in db.Users
                         where u.Email == email
                         select u;

            if (result.Count() == 0)
            {
                MembershipCreateStatus createStatus;
                Membership.CreateUser(email, password, email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    MembershipUser account = Membership.GetUser(email);

                    User user = new User();
                    user.Email = account.Email;
                    user.RegistrationDate = account.CreationDate;
                    user.PaymentId = Guid.NewGuid().ToString("N");
                    user.ActivationKey = Guid.NewGuid().ToString("N");

                    if (ModelState.IsValid)
                    {
                        db.Users.Add(user);
                        db.SaveChanges();

                        Roles.AddUserToRole(account.Email, "User");
                    }
                    else
                    {
                        Membership.DeleteUser(account.Email);
                        flag = false;
                    }
                }
            }

            if (flag)
            {
                FormsAuthentication.SetAuthCookie(email, false /* createPersistentCookie */);
            }

            return RedirectToAction("Index", "Home");

        }


        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Użytkownik o takiej nazwie już istnieje.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Na ten adres email założono już konto.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Podane hasło jest niepoprawne.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Podany adres email jest niepoprawny.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Podany adres email jest niepoprawny.";

                case MembershipCreateStatus.ProviderError:
                    return "Wystąpił problem po stronie usługodawcy. Jeśli sytuacja będzie się powtarzać skontaktuj się z administracją.";

                case MembershipCreateStatus.UserRejected:
                    return "Proces tworzenia użytkownika został przerwany. Jeśli sytuacja będzie się powtarzać skontaktuj się z administracją.";

                default:
                    return "Wystąpił nieznany błąd. Jeśli sytuacja będzie się powtarzać skontaktuj się z administracją.";
            }
        }
        #endregion

        [Authorize]
        public ActionResult MyProfile()
        {
            DatabaseContext db = new DatabaseContext();
         
            var user = (from s in db.Users where s.Email == User.Identity.Name select s).First();

            user.PremiumEnd = DateTime.Now + new TimeSpan(10, 10, 10, 10);
            db.SaveChanges();

            ViewBag.AccountType = user.IsPremiumAccount();

            if (user.IsPremiumAccount())
            {
                ViewBag.EndDate = user.PremiumEnd.ToString();
            }
            
            
            return View();
        }
    }

}
