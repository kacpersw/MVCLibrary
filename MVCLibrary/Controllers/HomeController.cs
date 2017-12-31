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
            var books = dbContext.Book.ToList();

            List<BookToShowViewModel> bookvm = new List<BookToShowViewModel>(6);

            

            HomeViewModel vm = new HomeViewModel
            {
                Books = dbContext.Book.ToList(),
                Messages = dbContext.AdminMessage.Where(m=>m.MainPage==true).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult SearchBooks(string value)
        {
            var books = dbContext.Book.Where(b => b.ISBN == value || b.Title == value || b.Author == value).ToList();

            List<BookToShowViewModel> vm = new List<BookToShowViewModel>();

            foreach (var book in books)
            {
                vm.Add(new BookToShowViewModel
                {
                    Author = book.Author,
                    CountBooks = book.CountBooks,
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Category = dbContext.Category.Where(c=>c.Id == book.CategoryId).FirstOrDefault().NameOfCategory
                });
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult SearchBooksByCategory(string value)
        {
            var category = dbContext.Category.Where(c => c.NameOfCategory == value).FirstOrDefault();

            if (category == null)
                return RedirectToAction("Index", "Home");

            var books = dbContext.Book.Where(b => b.CategoryId == category.Id).ToList();

            List<BookToShowViewModel> vm = new List<BookToShowViewModel>();

            foreach (var book in books)
            {
                vm.Add(new BookToShowViewModel
                {
                    Author = book.Author,
                    CountBooks = book.CountBooks,
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory
                });
            }

            return View("SearchBooks",vm);
        }

        public ActionResult AllBooks()
        {
            var books = dbContext.Book.ToList();

            List<BookToShowViewModel> vm = new List<BookToShowViewModel>();

            foreach (var book in books)
            {
                vm.Add(new BookToShowViewModel
                {
                    Author = book.Author,
                    CountBooks = book.CountBooks,
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory
                });
            }

            return View("SearchBooks", vm);
        }
    }
}
