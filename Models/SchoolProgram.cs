using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend.Models
{
    public class SchoolProgram
    {
        public int SchoolProgramId { get; set; }
        [BindRequired]
        public string ProgramName { get; set; }
        public ProgramHead ProgramHead { get; set; }

        [BindNever]
        public ICollection<Subject> Subjects { get; set; }
    }
}
