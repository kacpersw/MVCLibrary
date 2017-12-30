using MVCLibrary.Models;
using MVCLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryEntities dbContext;

        public HomeController()
        {
            this.dbContext = new LibraryEntities();
        }

        public ActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel
            {
                Books = dbContext.Book.ToList(),
                Messages = dbContext.AdminMessage.ToList()
            };

            return View(vm);
        }
    }
}
