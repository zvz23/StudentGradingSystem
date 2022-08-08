using Microsoft.EntityFrameworkCore;

namespace backend.Models
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class ProgramHead
    {
        public int ProgramHeadId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int SchoolProgramId { get; set; }
        public SchoolProgram SchoolProgram { get; set; }
    }
}
