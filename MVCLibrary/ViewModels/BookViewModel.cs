using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.ViewModels
{
    public class BookViewModel
    {
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Display(Name = "Ilość książek")]
        public int CountBooks { get; set; }

        [Display(Name = "Kategoria")]
        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual int CategoryId { get; set; }
    }
}