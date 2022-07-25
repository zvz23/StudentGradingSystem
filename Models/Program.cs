using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend.Models
{
    public class Program
    {
        public int ProgramId { get; set; }
        [BindRequired]
        public string ProgramName { get; set; }
        [BindRequired]

        public string ProgramHead { get; set; }

        [BindNever]
        public ICollection<Subject> Subjects { get; set; }
    }
}
