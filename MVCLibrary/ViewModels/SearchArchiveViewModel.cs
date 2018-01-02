using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCLibrary.ViewModels
{
    public class SearchArchiveViewModel
    {
        [Display(Name = "Tytuł wyszukiwania")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Data wyszukiwania")]
        public DateTime Date { get; set; }

        public List<BookToShowViewModel> BookList { get; set; }

    }
}