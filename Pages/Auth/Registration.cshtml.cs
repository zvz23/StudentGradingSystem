using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Forms;
using backend.Forms.Helpers;
using backend.Models;


namespace backend.Pages
{
    public class RegistrationModel : PageModel
    {

        public RegistrationModel(GradingContext context, ILogger<RegistrationModel> logger) {
            _context = context;
            _logger = logger;
        }

        private readonly GradingContext _context;
        private readonly ILogger<RegistrationModel> _logger;

        [BindProperty]
        public RegistrationForm SignupForm { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost() {
            if (ModelState.IsValid) {
                StudentAccount studentAccount = StudentAccountHelper.MapToStudentAccount(SignupForm);
                await _context.Students.AddAsync(studentAccount);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("signup_message", "Your registration has been successfully completed. Please wait for the Program Head to approve your registration.");
                return RedirectToPage("Registration");
            }
            return Page();

        }

        
    }
}
