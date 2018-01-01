using MVCLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCLibrary.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<BookToShowViewModel> Books { get; set; }
        public IEnumerable<AdminMessage> Messages { get; set; }
    }
}