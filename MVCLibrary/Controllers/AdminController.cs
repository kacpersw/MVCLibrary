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

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WorkersToVerify()
        {

            var workers = dbContext.Users
                .Where(u => u.Role == "Worker")
                .ToList();

            return View(workers);
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddMessage()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddMessage(AdminMessage message)
        {
            if (ModelState.IsValid)
            {
                message.MainPage = true;
                dbContext.AdminMessage.Add(message);
                dbContext.SaveChanges();
            }
            else
            {
                return View();
            }


            return RedirectToAction("Index", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ManageMessages()
        {
            var vm = dbContext.AdminMessage.ToList();

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddToMainPage(int id)
        {
            var message = dbContext.AdminMessage.Where(m => m.Id == id).FirstOrDefault();

            message.MainPage = true;
            dbContext.SaveChanges();

            return RedirectToAction("ManageMessages", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveFromMainPage(int id)
        {
            var message = dbContext.AdminMessage.Where(m => m.Id == id).FirstOrDefault();

            message.MainPage = false;
            dbContext.SaveChanges();

            return RedirectToAction("ManageMessages", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveMessage(int id)
        {
            var message = dbContext.AdminMessage.Where(m => m.Id == id).FirstOrDefault();

            dbContext.AdminMessage.Remove(message);
            dbContext.SaveChanges();

            return RedirectToAction("ManageMessages", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddLimit()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddLimit(Limit limit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var l = dbContext.Limit.FirstOrDefault();

                    if (l != null)
                        dbContext.Limit.Remove(l);

                    dbContext.Limit.Add(limit);
                    dbContext.SaveChanges();
                }
                else
                {
                    return View();
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}