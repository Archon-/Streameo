using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Streameo.Controllers
{
    public class StreamingController : Controller
    {
        //
        // GET: /Streaming/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listen(int id)
        {
            string file = "";
            if(id == 1)
                file = Server.MapPath("~/Music/Tesserakt/The Uknown/03 111.mp3");
            else if (id == 2)
                file = Server.MapPath("~/Music/Tesserakt/The Uknown/01 Ultrarozpiedalator.mp3");

            using(var file1 = new FileStream(file, FileMode.Open))
            {
                using(var stream = new MemoryStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = file1.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, read);
                        
                    }
                    //return buffer;
                    //return null;
                    //stream.Position = 0;
                    //return stream.
                }
            }


            return File(file, "audio/mp3");
            
        }

        //
        // GET: /Streaming/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Streaming/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Streaming/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Streaming/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Streaming/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Streaming/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Streaming/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
