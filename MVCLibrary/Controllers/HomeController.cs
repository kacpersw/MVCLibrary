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

        public ActionResult UserPanel()
        {
            return View();
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

            var userEmail = User.Identity.Name;
            var user = dbContext.Users.Where(u => u.EmailID == userEmail).FirstOrDefault();

            dbContext.Search.Add(new Search
            {
                UserId = user.UserID,
                Title = value,
                DateOfSearch = DateTime.Now
            });
            dbContext.SaveChanges();

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
            var userEmail = User.Identity.Name;
            var user = dbContext.Users.Where(u => u.EmailID == userEmail).FirstOrDefault();

            dbContext.Search.Add(new Search
            {
                UserId = user.UserID,
                Title = value,
                DateOfSearch = DateTime.Now
            });
            dbContext.SaveChanges();

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

        [Authorize]
        public ActionResult AddProductToCart(Cart cart, int id, int quantity = 1)
        {
            Book item = dbContext.Book.Where(b => b.Id == id).FirstOrDefault();
            cart.AddItem(item);
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult RemoveProductFromCart(Cart cart, int id)
        {
            Book item = dbContext.Book.Where(b => b.Id == id).FirstOrDefault();
            cart.RemoveItem(item);

            return RedirectToAction("Cart");
        }

        [Authorize]
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
            var userObject = dbContext.Users.Where(u => u.EmailID == user).FirstOrDefault();
            var userId = userObject.UserID;
            var limit = dbContext.Limit.FirstOrDefault();

            if (userObject.UserBooks + cart.Books.Count > limit.CountOfBooks)
            {
                ViewBag.Message = "Nie możesz wypożyczyć tylu książek, ponieważ przekraczasz limit";
                return RedirectToAction("Cart", "Home");
            }

            foreach (var book in cart.Books)
            {
                BookSpecimen bs = dbContext.BookSpecimen.Where(b => b.BookId == book.Id && b.StatusOfBook == "Magazyn").FirstOrDefault();
                bs.StatusOfBook = "Zamówiona";
                var bookToUpdate = dbContext.Book.Where(b => b.Id == book.Id).FirstOrDefault();

                bookToUpdate.BooksInLibrary -= 1;
                dbContext.SaveChanges();

                dbContext.Borrow.Add(new Borrow
                {
                    BorrowDate = DateTime.Now,
                    BorrowState = "Zgłoszono",
                    BookId = bs.Id,
                    UserId = userId,
                });
            }

            userObject.UserBooks += cart.Books.Count;
            dbContext.SaveChanges();
            cart = new Cart();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult UserBorrow()
        {
            var user = User.Identity.Name;
            var userId = dbContext.Users.Where(u => u.EmailID == user).FirstOrDefault().UserID;

            List<BorrowViewModel> vm = new List<BorrowViewModel>();

            var borrows = dbContext.Borrow.Where(b => b.UserId == userId).ToList();

            foreach (var borrow in borrows)
            {
                var specimen = dbContext.BookSpecimen.Where(s => s.Id == borrow.BookId).FirstOrDefault();
                var book = dbContext.Book.Where(b => b.Id == specimen.BookId).FirstOrDefault();

                vm.Add(new BorrowViewModel
                {
                    Id = borrow.Id,
                    BookName = book.Title,
                    Author = book.Author,
                    BorrowDate = borrow.BorrowDate,
                    BorrowState = borrow.BorrowState,
                    Username = dbContext.Users.Where(u => u.UserID == borrow.UserId).FirstOrDefault().EmailID
                });
            }

            return View(vm);
        }

        [Authorize]
        public ActionResult GetSearchHistory()
        {
            var userEmail = User.Identity.Name;
            var user = dbContext.Users.Where(u => u.EmailID == userEmail).FirstOrDefault();

            var search = dbContext.Search.Where(s => s.UserId == user.UserID).ToList();
            //todo

            List<SearchArchiveViewModel> vm = new List<SearchArchiveViewModel>();
            foreach (var s in search)
            {
                var category = dbContext.Category.Where(c => c.NameOfCategory == s.Title).FirstOrDefault();
                int categoryId;
                if (category == null)
                {
                    categoryId = -1;
                }
                else
                {
                    categoryId = category.Id;
                }

                List<Book> books = dbContext.Book.Where(b => b.Title == s.Title || b.ISBN == s.Title || b.Author == s.Title || b.CategoryId == categoryId).ToList();
                List<BookToShowViewModel> booksViewModel = new List<BookToShowViewModel>();

                foreach (var book in books)
                {
                    booksViewModel.Add(new BookToShowViewModel
                    {
                        Author = book.Author,
                        BooksInLibrary = book.BooksInLibrary??0,
                        CountBooks = book.CountBooks,
                        Id = book.Id,
                        ISBN = book.ISBN,
                        Title = book.Title,
                        Category = dbContext.Category.Where(c=>c.Id == book.CategoryId).FirstOrDefault().NameOfCategory
                    });
                }
                vm.Add(new SearchArchiveViewModel
                {
                    Date = s.DateOfSearch??DateTime.Now,
                    Title = s.Title,
                    BookList = booksViewModel
                });

            }

            return View(vm);
        }
    }
}
