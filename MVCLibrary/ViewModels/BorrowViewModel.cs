using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCLibrary.ViewModels
{
    public class BorrowViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tytył")]
        public string BookName { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Data wypożyczenia")]
        public System.DateTime BorrowDate { get; set; }

        [Display(Name = "Użytkownik")]
        public string Username { get; set; }

        [Display(Name = "Status")]
        public string BorrowState { get; set; }
    }
}