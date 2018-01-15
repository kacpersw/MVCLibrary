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
        [Authorize(Roles = "Worker, Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult CategoriesList()
        {
            var categories = dbContext.Category.ToList();

            List<CategoryToShowViewModel> vm = new List<CategoryToShowViewModel>();
            foreach (var category in categories)
            {
                string parentName;
                var parent = dbContext.Category.Where(c => c.Id == category.ParentId).FirstOrDefault();

                if (parent == null)
                    parentName = "Brak";
                else
                    parentName = parent.NameOfCategory;

                vm.Add(new CategoryToShowViewModel
                {
                    Id = category.Id,
                    Name = category.NameOfCategory,
                    Parent = parentName
                });
            }
            return View(vm);
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult UsersToVerify()
        {
            if (User.IsInRole("Admin"))
            {
                var users = dbContext.Users
                .ToList();

                return View(users);
            }
            else
            {
                var users = dbContext.Users
                .Where(u => u.Role == "User")
                .ToList();

                return View(users);
            }           
        }

        [Authorize(Roles = "Worker, Admin")]
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

        [Authorize(Roles = "Worker, Admin")]
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

        [Authorize(Roles = "Worker, Admin")]
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

        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public ActionResult AddCategory(CategoryViewModel vm)
        {
            if (vm.ParentId == null)
                vm.ParentId = 0;
            
            if (ModelState.IsValid)
            {
                dbContext.Category.Add(new Category
                {
                    NameOfCategory = vm.Name,
                    ParentId = vm.ParentId
                });
            }
            else
            {
                var categories = dbContext.Category.ToList();
                vm.Parents = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.NameOfCategory
                });
                return View(vm);
            }
            


            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Worker, Admin")]
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

        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public ActionResult AddBook(BookViewModel vm)
        {
            if (ModelState.IsValid)
            {
                Book book = new Book
                {
                    Author = vm.Author,
                    CategoryId = vm.CategoryId,
                    CountBooks = vm.CountBooks,
                    ISBN = vm.ISBN,
                    Title = vm.Title,
                    BooksInLibrary = vm.CountBooks
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
            }
            else
            {
                var categories = dbContext.Category.ToList();

                var vm2 = new BookViewModel();

                vm2.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.NameOfCategory
                });
                return View(vm2);
            }
            

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult BooksList()
        {
            var books = dbContext.Book.ToList();

            List<BookToShowViewModel> vm = new List<BookToShowViewModel>();

            foreach (var book in books)
            {
                vm.Add(new BookToShowViewModel
                {
                    Id = book.Id,
                    Author = book.Author,
                    CountBooks = book.CountBooks,
                    ISBN = book.ISBN,
                    Title = book.Title,
                    Category = dbContext.Category.Where(c=>c.Id == book.CategoryId).FirstOrDefault().NameOfCategory
                });
            }

            return View(vm);
        }

        [Authorize(Roles = "Worker, Admin")]
        [HttpGet]
        public ActionResult AddSpecimen(int id)
        {
            var bookId = dbContext.Book.Where(b => b.Id == id).FirstOrDefault().Id;

            SpecimenViewModel vm = new SpecimenViewModel
            {
                Id = bookId,
                Count = 0
            };

            return View(vm);
        }

        [Authorize(Roles = "Worker, Admin")]
        [HttpPost]
        public ActionResult AddSpecimen(SpecimenViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    for (int i = 0; i < vm.Count; i++)
                    {
                        dbContext.BookSpecimen.Add(new BookSpecimen
                        {
                            BookId = vm.Id,
                            StatusOfBook = "Magazyn"
                        });
                    }

                    var book = dbContext.Book.Where(b => b.Id == vm.Id).FirstOrDefault();
                    book.CountBooks += vm.Count;
                    book.BooksInLibrary += vm.Count;
                    dbContext.SaveChanges();
                }
                else
                {
                    return View();
                }
            }
            catch(Exception e)
            {
                return RedirectToAction("AddSpecimen", "Worker");
            }

            return RedirectToAction("Index", "Worker");
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult BorrowsToVerify()
        {
            var borrows = dbContext.Borrow.Where(b => b.BorrowState == "Zgłoszono").ToList();
            List<BorrowViewModel> vm = new List<BorrowViewModel>();

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
                    Username = dbContext.Users.Where(u=>u.UserID == borrow.UserId).FirstOrDefault().EmailID
                });
            }

            return View(vm);
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult ReturnBook()
        {
            var borrows = dbContext.Borrow.Where(b => b.BorrowState == "Odebrano").ToList();
            List<BorrowViewModel> vm = new List<BorrowViewModel>();

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

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult ReturnedBook()
        {
            var borrows = dbContext.Borrow.Where(b => b.BorrowState == "Zwrócono").ToList();
            List<BorrowViewModel> vm = new List<BorrowViewModel>();

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

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult UserReturnedBook(int id)
        {
            var borrow = dbContext.Borrow.Where(b => b.Id == id).FirstOrDefault();
            var bookSpecimen = dbContext.BookSpecimen.Where(b => b.Id == borrow.BookId).FirstOrDefault();
            bookSpecimen.StatusOfBook = "Magazyn";
            var book = dbContext.Book.Where(b => b.Id == bookSpecimen.BookId).FirstOrDefault();
            book.BooksInLibrary += 1;

            borrow.BorrowState = "Zwrócono";
            dbContext.SaveChanges();

            return RedirectToAction("ReturnBook");
        }

        [Authorize(Roles = "Worker, Admin")]
        public ActionResult ChangeBorrowStatus(int id)
        {
            var borrow = dbContext.Borrow.Where(b => b.Id == id).FirstOrDefault();
            var bookSpecimen = dbContext.BookSpecimen.Where(b => b.Id == borrow.BookId).FirstOrDefault();
            bookSpecimen.StatusOfBook = "Czytelnik";

            borrow.BorrowState = "Odebrano";
            dbContext.SaveChanges();

            return RedirectToAction("BorrowsToVerify");
        }

        [Authorize(Roles = "Admin, Worker")]
        public ActionResult CheckUserBorrows(string emailID)
        {
            var userId = dbContext.Users.Where(u => u.EmailID == emailID).FirstOrDefault().UserID;

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
    }
}