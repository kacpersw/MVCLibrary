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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pole wymagane")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pole wymagane")]
        [Display(Name = "Tytuł")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pole wymagane")]
        [Display(Name = "Autor")]
        public string Author { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pole wymagane")]
        [Display(Name = "Ilość książek")]
        public int CountBooks { get; set; }

        [Display(Name = "Kategoria")]
        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual int CategoryId { get; set; }
    }
}