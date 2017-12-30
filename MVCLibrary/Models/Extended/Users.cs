    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCLibrary.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class Users
    {
        public string ConfirmPass { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "Imię")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Imię jest wymagane")]
        public string FirstName { get; set; }

        [Display(Name = "Nazwisko")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwisko jest wymagane")]
        public string LastName { get; set; }

        [Display(Name ="Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email jest wymagany")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "Hasło")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage = "Hasło musi się składać z minimum 6 znaków")]
        public string Pass { get; set; }

        [Display(Name = "Potwierdz hasło")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Potwierdzenie hasła jest wymagane")]
        [Compare("Pass", ErrorMessage = "Hasła są inne")]
        public string ConfirmPass { get; set; }

        [Display(Name = "Rola")]
        public string Role { get; set; }
        public Nullable<bool> IsUserVerified { get; set; }
        public Nullable<System.Guid> ActivationCode { get; set; }
    }
}