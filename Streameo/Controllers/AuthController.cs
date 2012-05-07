using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Streameo.Models;
using Facebook;
using System.Net;
using System.IO;
using System.Text;

namespace Streameo.Controllers
{ 
    public class AuthController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        //
        // GET: /Auth/

        public ViewResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        // GET: /Auth/Details/5

        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string pass)
        {
            pass = FormsAuthentication.HashPasswordForStoringInConfigFile(pass, "SHA1");
            
            var result = from u in db.Users
                         where u.Email == email && u.Pass == pass
                         select u;

            if (result.Count() > 0)
            {
                FormsAuthentication.SetAuthCookie(email, false);

                return RedirectToAction("Index");
            }

            return View();
        }

        public ActionResult Facebook()
        {
            return new RedirectResult("https://graph.facebook.com/oauth/authorize?type=web_server&client_id=144618862327767&redirect_uri=http://localhost:1188/auth/handshake/&scope=email%2Coffline_access%2Cuser_about_me");
        }

        [ActionName("handshake")]
        public ActionResult Handshake(string code)
        {
            //after authentication, Facebook will redirect to this controller action with a QueryString parameter called "code" (this is Facebook's Session key)

            //example uri: http://www.examplewebsite.com/facebook/handshake/?code=2.DQUGad7_kFVGqKTeGUqQTQ__.3600.1273809600-1756053625|dil1rmAUjgbViM_GQutw-PEgPIg.

            //this is your Facebook App ID
            string clientId = "144618862327767";

            //this is your Secret Key
            string clientSecret = "48af78235494ff833ed27d91d89a903d";

            //we have to request an access token from the following Uri
            string url = "https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}";

            //your redirect uri must be EXACTLY the same Uri that caused the initial authentication handshake
            string redirectUri = "http://localhost:1188/auth/handshake/";

            //Create a webrequest to perform the request against the Uri
            WebRequest request = WebRequest.Create(string.Format(url, clientId, redirectUri, clientSecret, code));

            //read out the response as a utf-8 encoding and parse out the access_token
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader streamReader = new StreamReader(stream, encode);
            string accessToken = streamReader.ReadToEnd().Replace("access_token=", "");
            streamReader.Close();
            response.Close();

            //set the access token to some session variable so it can be used through out the session
            Session["FacebookAccessToken"] = accessToken;

            var client = new FacebookClient(accessToken);
            dynamic me = client.Get("me");

            string email = me.email;

            var result = from u in db.Users
                         where u.Email == email
                         select u;

            if (result.Count() == 0)
            {
                User user = new User();

                user.Email = me.email;
                user.Pass = FormsAuthentication.HashPasswordForStoringInConfigFile("test", "SHA1");
                user.Name = me.name;
                user.PremiumStatus = false;
                user.RegistrationDate = DateTime.Now;
                user.SongsAdded = 1;
                user.TimeOfListening = 100;

                if (ModelState.IsValid)
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            FormsAuthentication.SetAuthCookie(email, false);

            return RedirectToAction("Index");

            //return Content(email);
        }

        public ViewResult Details(int id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // GET: /Auth/Create

        public ActionResult Register()
        {
            return View();
        } 

        //
        // POST: /Auth/Create

        [HttpPost]
        public ActionResult Register(User user)
        {
            user.Pass = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Pass, "SHA1");
            
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(user);
        }
        
        //
        // GET: /Auth/Edit/5
 
        public ActionResult Edit(int id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // POST: /Auth/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /Auth/Delete/5
 
        public ActionResult Delete(int id)
        {
            User user = db.Users.Find(id);
            return View(user);
        }

        //
        // POST: /Auth/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}