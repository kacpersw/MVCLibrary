using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCLibrary.ViewModels
{
    public class CategoryViewModel
    {
        public string Name { get; set; }

        [Display(Name = "Kategoria rodzica")]
        public IEnumerable<SelectListItem> Parents { get; set; }

        public virtual int ParentId { get; set; }
    }
}