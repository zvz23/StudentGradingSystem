
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
            new SelectListItem() { Value = "2", Text = "2" },
            new SelectListItem() { Value = "3", Text = "3" },
            new SelectListItem() { Value = "4", Text = "4" },
            new SelectListItem() { Value = "5", Text = "5" },
            new SelectListItem() { Value = "6", Text = "6" },
        };
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
                return RedirectToPage("Subjects");
            }
            LoadProperties();
            return Page();
        }


        private async Task LoadProperties()
        {
            Subjects = await _context.Subjects
                .Include(s => s.Prerequisite)
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
            return RedirectToPage("Subjects");
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            if (ModelState.IsValid)
            {
                _context.Subjects.Update(await SubjectForm.MapToSubject(_context));
                await _context.SaveChangesAsync();
                return RedirectToPage("Subjects");
            }
            await LoadProperties();
            return Page();
        }

        public async Task<IActionResult> OnGetSubject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Subject subject = await _context.Subjects
                .Include(s => s.Prerequisite)
                .ThenInclude(p => p.SubjectCodes)
                .FirstOrDefaultAsync(s => s.SubjectId == id);
            if (SubjectForm == null)
            {
                return NotFound();
            }
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

    public class SubjectForm
    {
        public int SubjectId { get; set; }
        [BindRequired]
        [Display(Name = "Code No.")]
        public string CodeNo { get; set; }

        [BindRequired]
        [Display(Name = "Descriptive Title")]
        public string DescriptiveTitle { get; set; }

        [BindRequired]
        public int Units { get; set; }

        [BindRequired]
        public ClassType ClassType { get; set; }

        [BindRequired]
        public PrerequisiteType PrerequisiteType { get; set; }
        [Display(Name = "Prerequisite Percentage")]
        public int? PrerequisitePercentage { get; set; }
        [Display(Name = "Prerequisite Subjects")]
        public List<int>? PrerequisiteSubjectCodes { get; set; }



        public async Task<Subject> MapToSubject(GradingContext context)
        {
            Subject subject = new Subject()
            {
                CodeNo = CodeNo,
                DescriptiveTitle = DescriptiveTitle,
                Units = Units,
                Type = ClassType,
                Prerequisite = new Prerequisite()
                {
                    Type = PrerequisiteType
                }
            };


            if (PrerequisiteType == PrerequisiteType.Subject)
            {
                if (PrerequisiteSubjectCodes == null || PrerequisiteSubjectCodes.Count < 1)
                {
                    throw new ArgumentException("PrerequisiteSubjectCodes is empty or null");
                }
                StringBuilder stringBuilder = new StringBuilder();
                

                foreach (var id in PrerequisiteSubjectCodes)
                {
                   
                    Subject preSub = await context.Subjects.FirstOrDefaultAsync(s => id == s.SubjectId);
                    if (preSub == null)
                    {
                        throw new ArgumentException($"Subject with the {id} subject code number does not exists");
                    }
                    stringBuilder.Append($"{preSub.CodeNo},");
                   

                }
                subject.Prerequisite.SubjectCodes = stringBuilder.ToString();
            }
            else if (PrerequisiteType == PrerequisiteType.TotalUnits)
            {
                subject.Prerequisite.Percentage = PrerequisitePercentage;
            }

            return subject;

        }
        


    }
}