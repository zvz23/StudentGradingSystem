
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Data;
using backend.Forms;
using backend.Forms.Helpers;

namespace backend.Pages {
    public class IndexModel : PageModel {
        public IndexModel(GradingContext context, ILogger<IndexModel> logger) {
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
        public Subject PartialSubject { get; set; }


        public List<Subject> Subjects { get; set; } = new();
        public async Task<IActionResult> OnGet() {
            if (_context.Subjects == null)
            {
                return BadRequest();
            }
            await LoadProperties();
            return Page();
        }

        public async Task<IActionResult> OnPostSave() {
            SubjectHelper.ValidatePrerequisites(SubjectForm, this);
            if (ModelState.IsValid) {
                Subject subject = await SubjectHelper.MapToSubject(SubjectForm, _context);
                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            await LoadProperties();
            return Page();
        }


        private async Task LoadProperties()
        {
            Subjects = await _context.Subjects
                .Include(s => s.Prerequisite)
                .ThenInclude(sp => sp.Subjects)
                .ToListAsync();
            SubjectCodes = Subjects.Select(s => new SelectListItem() { Value = s.SubjectId.ToString(), Text = s.CodeNo }).ToList();
        }

        public async Task<IActionResult> OnPostDelete([FromForm] int? subjectId)
        {
            if (subjectId == null)
            {
                return NotFound();
            }
         

            Subject subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
            {
                return NotFound();
            }
            _context.Remove(subject);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }



        public async Task<IActionResult> OnGetSubject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PartialSubject = await _context.Subjects
                .Include(s => s.Prerequisite)
                .ThenInclude(p => p.Subjects)
                .FirstOrDefaultAsync(s => s.SubjectId == id);
            if (PartialSubject == null)
            {
                return NotFound();
            }
            this.SubjectForm = new SubjectForm();
            SubjectForm.CodeNo = PartialSubject.CodeNo;
            SubjectForm.DescriptiveTitle = PartialSubject.DescriptiveTitle;

            return Partial("SubjectUpdateFormPartial", this);

        }
        
    }

}