//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCLibrary.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Limit
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pole wymagane")]
        public int CountOfBooks { get; set; }
    }
}
