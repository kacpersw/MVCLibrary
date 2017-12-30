using MVCLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.Controllers
{
    public class AdminController : Controller
    {
        private readonly LibraryEntities dbContext;

        public AdminController()
        {
            this.dbContext = new LibraryEntities();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WorkersToVerify()
        {

            var workers = dbContext.Users
                .Where(u => u.Role == "Worker")
                .ToList();

            return View(workers);
        }

        public ActionResult VerifyWorker(string emailID)
        {

            var worker = dbContext.Users
                .Where(w => w.EmailID == emailID)
                .FirstOrDefault();

            if (worker != null)
            {
                worker.IsUserVerified = true;
                worker.ConfirmPass = worker.Pass;
                dbContext.SaveChanges();
            }

            return View("Index");
        }

        public ActionResult RemoveWorker(string emailID)
        {

            var worker = dbContext.Users
                .Where(w => w.EmailID == emailID)
                .FirstOrDefault();

            if (worker != null)
            {
                worker.Role = "User";
                worker.ConfirmPass = worker.Pass;
                dbContext.SaveChanges();
            }

            return View("Index");
        }

        [HttpGet]
        public ActionResult AddMessage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMessage(AdminMessage message)
        {
            dbContext.AdminMessage.Add(message);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddLimit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddLimit(Limit limit)
        {
            try
            {
                var l = dbContext.Limit.FirstOrDefault();

                if (l != null)
                    dbContext.Limit.Remove(l);

                dbContext.Limit.Add(limit);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}