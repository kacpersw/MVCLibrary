using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCLibrary.ViewModels
{
    public class SpecimenViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Ilość")]
        public int Count {get;set;}
    }
}