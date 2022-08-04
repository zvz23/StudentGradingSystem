using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using backend.Models;
using backend.Data;

namespace backend.Forms.Helpers
{
    public class SubjectHelper
    {
        public static SubjectForm FromSubject(Subject subject)
        {
            SubjectForm subjectForm = new SubjectForm()
            {
                SubjectId = subject.SubjectId,
                CodeNo = subject.CodeNo,
                DescriptiveTitle = subject.DescriptiveTitle,
                Units = subject.Units,
                ClassType = subject.Type,
            };
            if (subject.Prerequisite != null)
            {
                switch (subject.Prerequisite.Type)
                {
                    case PrerequisiteType.TotalUnits:
                        subjectForm.PrerequisiteType = PrerequisiteType.TotalUnits;
                        subjectForm.PrerequisitePercentage = subject.Prerequisite.Percentage;
                        break;
                    case PrerequisiteType.Subject:
                        subjectForm.PrerequisiteType = PrerequisiteType.Subject;
                        subjectForm.PrerequisiteSubjectIds = subject.Prerequisite.Subjects.Select(s => s.SubjectId).ToList();
                        break;
                    default:
                        subjectForm.PrerequisiteType = subject.Prerequisite.Type;
                        break;
                }
            }
            else
            {
                subjectForm.PrerequisiteType = PrerequisiteType.None;
            }
            return subjectForm;

        }

        public static async Task<Subject> MapToSubject(SubjectForm form, GradingContext context)
        {
            Subject subject = new Subject()
            {
                CodeNo = form.CodeNo,
                DescriptiveTitle = form.DescriptiveTitle,
                Units = form.Units,
                Type = form.ClassType,
                Prerequisite = new Prerequisite()
                {
                    Type = form.PrerequisiteType,
                    Subjects = new List<PrerequisiteSubject>()

                }
            };

            switch (form.PrerequisiteType)
            {
                case PrerequisiteType.Subject:
                    if (form.PrerequisiteSubjectIds == null || form.PrerequisiteSubjectIds.Count < 1)
                    {
                        throw new ArgumentException("PrerequisiteSubjectIds is empty or null");
                    }
                    foreach (var id in form.PrerequisiteSubjectIds)
                    {
                        Subject preSub = await context.Subjects.FirstOrDefaultAsync(s => id == s.SubjectId);
                        if (preSub == null)
                        {
                            throw new ArgumentException($"Subject with the {id} subject code number does not exists");
                        }
                        subject.Prerequisite.Subjects.Add(new PrerequisiteSubject()
                        {
                            SubjectId = preSub.SubjectId,
                        });
                    }
                    break;
                case PrerequisiteType.TotalUnits:
                    subject.Prerequisite.Percentage = form.PrerequisitePercentage;
                    break;
                default:
                    break;
            }

            return subject;
        }
        public static void ValidatePrerequisites(SubjectForm form, PageModel pageModel)
        {
            switch (form.PrerequisiteType)
            {
                case PrerequisiteType.Subject:
                    if (form.PrerequisiteSubjectIds == null || form.PrerequisiteSubjectIds.Count < 1)
                    {
                        pageModel.ModelState.AddModelError("SubjectForm.PrerequisiteSubjectIds", "Please select at least 1 prerequisite subject");
                    }
                    break;
                case PrerequisiteType.TotalUnits:
                    if (form.PrerequisitePercentage == null)
                    {
                        pageModel.ModelState.AddModelError("SubjectForm.PrerequisitePercentage", "Please enter a valid percentage");
                    }
                    break;
            }
        }

        public static async Task<List<PrerequisiteSubject>?> GetPrerequisiteSubjectsFromIds(IEnumerable<int> subjectIds, GradingContext context)
        {
            var prerequisiteSubjects = new List<PrerequisiteSubject>();
            foreach (var id in subjectIds)
            {
                Subject subject = await context.Subjects.FindAsync(id);
                if (subject == null)
                {
                    continue;
                }
                prerequisiteSubjects.Add(new PrerequisiteSubject()
                {
                    SubjectId = subject.SubjectId
                });
            }
            return prerequisiteSubjects;
        }

        
    }
}
