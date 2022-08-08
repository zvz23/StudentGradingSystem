using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace backend.Models {

    public class Subject {
        public int SubjectId { get; set; }
        public string CodeNo { get; set; }
        public string DescriptiveTitle { get; set; }
        public int Units { get; set; }
        public ClassType ClassType { get; set; }
        public Year Year { get; set; }
        public Semester Semester { get; set; }
        public int SchoolProgramId { get; set; }
        public SchoolProgram SchoolProgram { get; set; }

        public Prerequisite? Prerequisite { get; set; }

    }

    public class SubjectGrade
    {
        public int SubjectGradeId { get; set; }
        [Range(1.0, 5.0)]
        public decimal Grade { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

    }

    public class PrerequisiteSubject
    {
        public int PrerequisiteSubjectId { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int PrerequisiteId { get; set; }
        public Prerequisite Prerequisite { get; set; }
    }

    public class Prerequisite
    {
        public int PrerequisiteId { get; set; }
        public PrerequisiteType Type { get; set; }
        public List<PrerequisiteSubject>? Subjects{ get; set; }
        public int? Percentage { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public override string ToString()
        {
            string prerequisiteStr = string.Empty;
            switch(Type)
            {
                case PrerequisiteType.None:
                    break;
                case PrerequisiteType.Subject:
                    prerequisiteStr = string.Join(", ", Subjects.Select(ps => ps.Subject).Select(s => s.CodeNo));
                    break;
                case PrerequisiteType.TotalUnits:
                    prerequisiteStr = Percentage.ToString();
                    break;
                case PrerequisiteType.RegularStanding:
                case PrerequisiteType.Standing:
                    prerequisiteStr = Type.ToString();
                    break;
                default:
                    break;
            }
            return prerequisiteStr;
        }

    }
    
    public enum PrerequisiteType
    {
        None,
        Subject,
        TotalUnits,
        Standing,
        RegularStanding
    }

    public enum ClassType
    {
        LEC,
        LAB
    }

    public enum Semester {
        FIRST = 1 ,
        SECOND = 2
    }

    public enum Year {
        FIRST = 100,
        SECOND = 200,
        THIRD = 300,
        MIDYEAR = 350,
        FOURTH = 400,
    }

    
}


