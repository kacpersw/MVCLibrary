using MVCLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibrary.CartService
{
    public class Cart
    {
        public Cart()
        {
            Books = new List<Book>();
        }

        public List<Book> Books { get; set; }

        public void AddItem(Book book)
        {
            Books.Add(book);
        }

        public void RemoveItem(Book book)
        {
            var b = Books.FirstOrDefault(x => x.Id == book.Id);

            if (b != null)
                Books.Remove(b);
        }
    }
}