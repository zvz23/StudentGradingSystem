
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using backend.Models;
using backend.Data;
using System.Text;
using backend.Forms;

namespace backend.Pages {
    public class SubjectsModel : PageModel {
        public SubjectsModel(GradingContext context, ILogger<SubjectsModel> logger) {
            _context = context;
            _logger = logger;
        }
        private readonly ILogger<SubjectsModel> _logger;
        private readonly GradingContext _context;
        

        [BindProperty]
        public SubjectForm SubjectForm { get; set; }
        public List<SelectListItem> SubjectCodes { get; set; }
        public List<SelectListItem> SubjectTypes = Enum.GetNames(typeof(ClassType)).Select(ct => new SelectListItem() { Value = ct, Text = ct }).ToList();
        public List<SelectListItem> PrerequisiteTypes = Enum.GetNames(typeof(PrerequisiteType)).Select(pt => new SelectListItem() { Value = pt, Text = pt }).ToList();
        public List<SelectListItem> SelectUnits = new List<SelectListItem>() {
            new SelectListItem() { Value = "1", Text = "1" },
            new SelectListItem() { Value = "2", Text = "2"},
            new SelectListItem() { Value = "3", Text = "3", Selected = true },
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
            ValidatePrerequisites(SubjectForm, this);
            if (ModelState.IsValid) {
                Subject subject = await SubjectForm.MapToSubject(_context);
                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync();
                return RedirectToPage("Create");
            }
            LoadProperties();
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
            return RedirectToPage("Create");
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
        private void ValidatePrerequisites(SubjectForm form, PageModel pageModel)
        {
            switch (form.PrerequisiteType)
            {
                case PrerequisiteType.Subject:
                    if (form.PrerequisiteSubjectCodes == null || form.PrerequisiteSubjectCodes.Count < 1)
                    {
                        pageModel.ModelState.AddModelError("All", "Please select at least 1 prerequisite subject");
                    }
                    break;
                case PrerequisiteType.TotalUnits:
                    if (form.PrerequisitePercentage == null)
                    {
                        pageModel.ModelState.AddModelError("All", "Please enter a valid percentage");
                    }
                    break;
            }
        }
    }

}