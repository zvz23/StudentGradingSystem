using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using backend.Models;

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
        public int ProgramId { get; set;}
        [BindRequired]
        public Semester Semester { get; set; }
        [BindRequired]
        public Year Year { get; set; }

        [BindRequired]
        public PrerequisiteType PrerequisiteType { get; set; }
        [Display(Name = "Prerequisite Percentage")]
        public int? PrerequisitePercentage { get; set; }
        [Display(Name = "Prerequisite Subjects")]
        public List<int>? PrerequisiteSubjectIds { get; set; }

        

        
    }
}
