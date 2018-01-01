using MVCLibrary.CartService;
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

            List<Book> list = new List<Book>();
            foreach (var book in books)
            {
                list.Add(book);
            }
            list = list.OrderBy(b => b.Id).ToList();

            var counter = 0;
            foreach (var book in list)
            {
                bookvm.Add(new BookToShowViewModel
                {
                    Author = book.Author,
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory,
                    CountBooks = book.CountBooks,
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    BooksInLibrary = book.BooksInLibrary ?? 0
                });
                counter++;
                if (counter == 6)
                    break;
            }

            HomeViewModel vm = new HomeViewModel
            {
                Books = bookvm,
                Messages = dbContext.AdminMessage.Where(m => m.MainPage == true).ToList()
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
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory,
                    BooksInLibrary = book.BooksInLibrary ?? 0
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
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory,
                    BooksInLibrary = book.BooksInLibrary ?? 0
                });
            }

            return View("SearchBooks", vm);
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
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory,
                    BooksInLibrary = book.BooksInLibrary ?? 0
                });
            }

            return View("SearchBooks", vm);
        }

        public ActionResult AddProductToCart(Cart cart, int id, int quantity = 1)
        {
            Book item = dbContext.Book.Where(b => b.Id == id).FirstOrDefault();
            cart.AddItem(item);
            return RedirectToAction("Index");
        }

        public ActionResult RemoveProductFromCart(Cart cart, int id)
        {
            Book item = dbContext.Book.Where(b => b.Id == id).FirstOrDefault();
            cart.RemoveItem(item);

            return RedirectToAction("Cart");
        }

        public ActionResult Cart(Cart cart)
        {
            List<BookToShowViewModel> vm = new List<BookToShowViewModel>();

            foreach (var book in cart.Books)
            {
                vm.Add(new BookToShowViewModel
                {
                    Author = book.Author,
                    Category = dbContext.Category.Where(c => c.Id == book.CategoryId).FirstOrDefault().NameOfCategory,
                    CountBooks = book.CountBooks,
                    Id = book.Id,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    BooksInLibrary = book.BooksInLibrary ?? 0
                });
            }

            return View(vm);
        }

        [Authorize]
        public ActionResult CreateBorrow(Cart cart)
        {
            var user = User.Identity.Name;
            var userId = dbContext.Users.Where(u => u.EmailID == user).FirstOrDefault().UserID;
            foreach (var book in cart.Books)
            {
                BookSpecimen bs = dbContext.BookSpecimen.Where(b => b.BookId == book.Id && b.StatusOfBook == "Magazyn").FirstOrDefault();

                dbContext.Borrow.Add(new Borrow
                {
                    BorrowDate = DateTime.Now,
                    BorrowState = "Zgłoszono",
                    BookId = bs.Id,
                    UserId =userId,
                });
            }
            dbContext.SaveChanges();

            return View();
        }
    }
}
