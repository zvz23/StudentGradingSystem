using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using backend.Data;
using backend.Models;
using backend.Forms;
using backend.Forms.Helpers;


namespace backend.Pages
{
    public class AddSubjectModel : PageModel
    {
        public AddSubjectModel(GradingContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }
        private readonly ILogger<IndexModel> _logger;
        private readonly GradingContext _context;
        

        [BindProperty]
        public SubjectForm SubjectForm { get; set; }
        public List<SelectListItem> SubjectCodes { get; set; }
        public List<SelectListItem> SubjectTypes = Enum.GetNames(typeof(ClassType)).Select(ct => new SelectListItem() { Value = ct, Text = ct }).ToList();
        public List<SelectListItem> PrerequisiteTypes = Enum.GetNames(typeof(PrerequisiteType)).Select(pt => new SelectListItem() { Value = pt, Text = pt }).ToList();
        public List<SelectListItem> SelectUnits = new List<SelectListItem>() {
            new SelectListItem() { Value = "1", Text = "1" },
            new SelectListItem() { Value = "2", Text = "2"},
            new SelectListItem() { Value = "3", Text = "3" },
            new SelectListItem() { Value = "4", Text = "4" },
            new SelectListItem() { Value = "5", Text = "5" },
            new SelectListItem() { Value = "6", Text = "6" },
        };
        public List<SelectListItem> SelectPrograms = new List<SelectListItem>() {};
        public List<SelectListItem> SelectSemesters = Enum.GetNames(typeof(Semester)).Select(s => new SelectListItem() { Value = s , Text = s + " SEMESETER"}).ToList();
        public List<SelectListItem> SelectYears = Enum.GetNames(typeof(Year)).Select(y => new SelectListItem() { Value = y, Text = y + " YEAR"}).ToList();
        public Subject PartialSubject { get; set; }
        public void OnGet()
        {
            SelectPrograms = _context.Programs.AsEnumerable().Select(p => new SelectListItem() { Text = p.ProgramName, Value = p.ProgramId.ToString()}).ToList();
            SubjectCodes  = _context.Subjects.AsEnumerable().Select(s => new SelectListItem() { Text = s.CodeNo, Value = s.SubjectId.ToString()}).ToList();
        }

        public async Task<IActionResult> OnPostSave() {
            SubjectHelper.ValidatePrerequisites(SubjectForm, this);
            if (ModelState.IsValid) {
                Subject subject = await SubjectHelper.MapToSubject(SubjectForm, _context);
                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
