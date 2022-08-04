using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace backend.Pages
{
    public class LoginModel : PageModel
    {
        public LoginForm SigninForm { get; set; }
        public void OnGet()
        {
        }


        public class LoginForm {

            [BindRequired]
            [Display(Name = "School Id")]
            public string SchoolId { get; set; }
            public string Password { get; set; }

        }
    }
}
