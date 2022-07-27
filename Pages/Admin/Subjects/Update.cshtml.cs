using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Forms;

namespace backend.Pages.Admin.Subjects
{
    public class UpdateModel : PageModel
    {
        public UpdateModel(GradingContext context, ILogger<GradingContext> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        private readonly GradingContext _context;
        private readonly ILogger<GradingContext> _logger;

        public List<SelectListItem> SubjectCodes { get; set; }
        public List<SelectListItem> SubjectTypes = Enum.GetNames(typeof(ClassType)).Select(ct => new SelectListItem() { Value = ct, Text = ct }).ToList();
        public List<SelectListItem> PrerequisiteTypes = Enum.GetNames(typeof(PrerequisiteType)).Select(pt => new SelectListItem() { Value = pt, Text = pt }).ToList();
        public List<SelectListItem> SelectUnits = new List<SelectListItem>() {
            new SelectListItem() { Value = "1", Text = "1" },
            new SelectListItem() { Value = "2", Text = "2" },
            new SelectListItem() { Value = "3", Text = "3" },
            new SelectListItem() { Value = "4", Text = "4" },
            new SelectListItem() { Value = "5", Text = "5" },
            new SelectListItem() { Value = "6", Text = "6" },
        };

        [BindProperty]
        public SubjectForm SubjectForm { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            Subject subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }
            LoadProperties();
            SubjectForm = SubjectForm.FromSubject(subject);
            return Page();
        }

        private async Task LoadProperties()
        {
            SubjectCodes = await _context.Subjects
                .Include(s => s.Prerequisite)
                .ThenInclude(sp => sp.Subjects)
                .Select(s => new SelectListItem() { Value = s.SubjectId.ToString(), Text = s.CodeNo }).ToListAsync();
        }



    }
}