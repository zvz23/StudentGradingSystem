using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Forms;
using backend.Forms.Helpers;

namespace backend.Pages.Admin.Subject {
public class UpdateSubjectModel : PageModel {

    public UpdateSubjectModel(GradingContext context, ILogger<UpdateSubjectModel> logger) {
        _context = context;
        _logger = logger;

    }

    private readonly GradingContext _context;
    private readonly ILogger<UpdateSubjectModel> _logger;

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

        public backend.Models.Subject PartialSubject { get; set; }
    public async Task<IActionResult> OnGet([FromQuery] int? subjectId) {
        if (subjectId != null) {
            _logger.LogInformation("THE SUBJECT ID IS " + subjectId.ToString());

        }
        else {
            _logger.LogInformation("THE SUBJECT ID IS NULLL" + subjectId.ToString());

        }
        if (subjectId == null) {
            return BadRequest("REQUEST IS BAD");
        }
        PartialSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == subjectId);
        if (PartialSubject == null) {
            return NotFound("REQUEST IS NOT FOUND");
        }
        SubjectForm = SubjectHelper.FromSubject(PartialSubject);

        return Page();
    }

    public async Task<IActionResult> OnPost() {

        if (ModelState.IsValid) {
            backend.Models.Subject subject = await _context.Subjects
            .Include(s => s.Prerequisite)
            .Where(s => s.SubjectId == SubjectForm.SubjectId)
            .FirstAsync();
            if (subject == null) {
                return NotFound();
            }
            SchoolProgram schoolProgram = await _context.Programs.FindAsync(SubjectForm.ProgramId);
            if (schoolProgram == null) {
                return NotFound();
            }
            subject.CodeNo = SubjectForm.CodeNo;
            subject.DescriptiveTitle = SubjectForm.DescriptiveTitle;
            subject.Units = SubjectForm.Units;
            subject.ClassType = SubjectForm.ClassType;
            subject.Year = SubjectForm.Year;
            subject.Semester = SubjectForm.Semester;
            subject.SchoolProgram = schoolProgram;
            if (subject.Prerequisite == null) {
                subject.Prerequisite = new Prerequisite();
                subject.Prerequisite.Type = SubjectForm.PrerequisiteType;
            }
            switch (subject.Prerequisite.Type)
            {
                case PrerequisiteType.Subject:
                    if (SubjectForm.PrerequisiteSubjectIds == null || SubjectForm.PrerequisiteSubjectIds.Count < 1)
                    {
                        throw new ArgumentException("PrerequisiteSubjectIds is empty or null");
                    }
                    foreach (var id in SubjectForm.PrerequisiteSubjectIds)
                    {
                        backend.Models.Subject preSub = await _context.Subjects.FindAsync(id);
                        if (preSub == null)
                        {
                            throw new ArgumentException($"Subject with the {id} subject code number does not exists");
                        }
                        subject.Prerequisite.Subjects.Add(new PrerequisiteSubject()
                        {
                            Subject = preSub
                        });
                    }
                    break;
                case PrerequisiteType.TotalUnits:
                    subject.Prerequisite.Percentage = SubjectForm.PrerequisitePercentage;
                    break;
                default:
                    break;
            }

            return RedirectToPage("/Admin/Subject/Index");

        }
        return Page();
    }



}
}
