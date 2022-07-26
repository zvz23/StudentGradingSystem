using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace backend.Models {

    public class Subject {
        public int SubjectId { get; set; }
        public string CodeNo { get; set; }
        public string DescriptiveTitle { get; set; }
        public int Units { get; set; }
        public ClassType Type { get; set; }
        public List<Program> Programs { get; set; }
        public Prerequisite? Prerequisite { get; set; }

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

    
}


