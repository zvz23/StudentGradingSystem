using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [Index(nameof(SchoolId), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class StudentAccount
    {
        public int StudentAccountId { get; set; }
        public string SchoolId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; } = false;
        public int SchoolProgramId { get; set; }
        public SchoolProgram SchoolProgram { get; set; }
        public ICollection<SubjectGrade> Grades { get; set; }

    }
}
