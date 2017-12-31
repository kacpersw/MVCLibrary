using MVCLibrary.Models;
using MVCLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.Controllers
{
    public class WorkerController : Controller
    {
        private readonly LibraryEntities dbContext;

        public WorkerController()
        {
            this.dbContext = new LibraryEntities();
        }

        // GET: Worker
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UsersToVerify()
        {

            var users = dbContext.Users
                .Where(u => u.Role == "User")
                .ToList();

            return View(users);
        }

        public ActionResult VerifyUsers(string emailID)
        {

            var user = dbContext.Users
                .Where(u => u.EmailID == emailID)
                .FirstOrDefault();

            if (user != null)
            {
                user.IsUserVerified = true;
                user.ConfirmPass = user.Pass;
                dbContext.SaveChanges();
            }

            return View("Index");
        }

        public ActionResult RemoveUser(string emailID)
        {

            var user = dbContext.Users
                .Where(u => u.EmailID == emailID)
                .FirstOrDefault();

            if (user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }

            return View("Index");
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            var categories = dbContext.Category.ToList();

            var vm = new CategoryViewModel();
            
            vm.Parents = categories.Select(c=> new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameOfCategory
            });


            return View(vm);
        }

        [HttpPost]
        public ActionResult AddCategory(CategoryViewModel vm)
        {
            dbContext.Category.Add(new Category
            {
                NameOfCategory = vm.Name,
                ParentId = vm.ParentId
            });


            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddBook()
        {
            var categories = dbContext.Category.ToList();

            var vm = new BookViewModel();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.NameOfCategory
            });

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddBook(BookViewModel vm)
        {
            Book book = new Book
            {
                Author = vm.Author,
                CategoryId = vm.CategoryId,
                CountBooks = vm.CountBooks,
                ISBN = vm.ISBN,
                Title = vm.Title
            };

            dbContext.Book.Add(book);

            for (int i = 0; i < vm.CountBooks; i++)
            {
                dbContext.BookSpecimen.Add(new BookSpecimen
                {
                    BookId = book.Id,
                    StatusOfBook = "Magazyn"
                });
            }

            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}