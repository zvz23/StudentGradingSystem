using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Data;

namespace backend.Forms
{
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
        [Display(Name = "Class Type")]
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
                    Type = PrerequisiteType,
                    Subjects = new List<PrerequisiteSubject>()

                }
            };

            if (PrerequisiteType != PrerequisiteType.None)
            {
                subject.Prerequisite = new Prerequisite();
                subject.Prerequisite.Type = PrerequisiteType;

                if (PrerequisiteType == PrerequisiteType.Subject)
                {
                    if (PrerequisiteSubjectCodes == null || PrerequisiteSubjectCodes.Count < 1)
                    {
                        throw new ArgumentException("PrerequisiteSubjectCodes is empty or null");
                    }
                    subject.Prerequisite.Subjects = new List<PrerequisiteSubject>();

                    foreach (var id in PrerequisiteSubjectCodes)
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
                }
                else if (PrerequisiteType == PrerequisiteType.TotalUnits)
                {
                    subject.Prerequisite.Percentage = PrerequisitePercentage;
                }
            }


            return subject;

        }

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
                switch(subject.Prerequisite.Type)
                {
                    case PrerequisiteType.TotalUnits:
                        subjectForm.PrerequisiteType = PrerequisiteType.TotalUnits;
                        subjectForm.PrerequisitePercentage = subject.Prerequisite.Percentage;
                        break;
                    case PrerequisiteType.Subject:
                        subjectForm.PrerequisiteType = PrerequisiteType.Subject;
                        subjectForm.PrerequisiteSubjectCodes = subject.Prerequisite.Subjects.Select(s => s.SubjectId).ToList();
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
    }
}
