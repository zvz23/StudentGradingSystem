using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Forms;
using backend.Forms.Helpers;

namespace backend.Pages
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
            Subject subject = await _context.Subjects
            .Where(s => s.SubjectId == id)
            .Include(s => s.Prerequisite)
            .ThenInclude(p => p.Subjects)
            .FirstAsync();
            if (subject == null)
            {
                return NotFound();
            }
            await LoadProperties(subject);
            SubjectForm = SubjectHelper.FromSubject(subject);
            return Page();
        }

        public async Task<IActionResult> OnPost() {
            if (SubjectForm.SubjectId == null)
            {
                return BadRequest();
            }

            SubjectHelper.ValidatePrerequisites(SubjectForm, this);
            if (ModelState.IsValid) {
                Subject toUpdateSubject = await _context.Subjects
                    .Where(s => s.SubjectId == SubjectForm.SubjectId)
                    .Include(s => s.Prerequisite)
                    .ThenInclude(p => p.Subjects)
                    .FirstAsync();

                if (toUpdateSubject == null) {
                    return NotFound();
                }
                toUpdateSubject.CodeNo = SubjectForm.CodeNo;
                toUpdateSubject.DescriptiveTitle = SubjectForm.DescriptiveTitle;
                toUpdateSubject.Units = SubjectForm.Units;
                toUpdateSubject.Type = SubjectForm.ClassType;
                toUpdateSubject.Prerequisite.Type = SubjectForm.PrerequisiteType;

                switch (SubjectForm.PrerequisiteType)
                {
                    case PrerequisiteType.Subject:
                        List<PrerequisiteSubject> prerequisiteSubjects = await SubjectHelper.GetPrerequisiteSubjectsFromIds(SubjectForm.PrerequisiteSubjectIds, _context);
                        toUpdateSubject.Prerequisite.Subjects = prerequisiteSubjects;
                        break;
                    case PrerequisiteType.TotalUnits:
                        toUpdateSubject.Prerequisite.Percentage = SubjectForm.PrerequisitePercentage;
                        break;
                    default:
                        break;

                }
                _context.Subjects.Update(toUpdateSubject);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            return Page();
        }

        private async Task LoadProperties(Subject subject)
        {
            SubjectCodes = await _context.Subjects
                .Where(s => s.SubjectId != subject.SubjectId)
                .Include(s => s.Prerequisite)
                .ThenInclude(sp => sp.Subjects)
                .Select(s => new SelectListItem() { Value = s.SubjectId.ToString(), Text = s.CodeNo }).ToListAsync();
        
            PrerequisiteTypes = Enum.GetNames(typeof(PrerequisiteType)).Select(pt => new SelectListItem() { Value = pt, Text = pt, Selected = pt == subject.Prerequisite.Type.ToString() }).ToList();
        }



    }
}
