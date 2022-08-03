using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace backend.Forms {

    public class RegistrationForm {
            [BindRequired]
            [Display(Name = "School Id")]
            public string SchoolId { get; set; }
            [BindRequired]
            [Required(ErrorMessage = "The First name field is required")]

            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [BindRequired]
            [Required(ErrorMessage = "The Last name field is required")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [BindRequired]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
            [BindRequired]
            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string PasswordConfirm { get; set; }
            [BindRequired]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
        }
}