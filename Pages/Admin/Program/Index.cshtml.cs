using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Data;

namespace backend.Pages.Admin.Program
{
    public class IndexModel : PageModel
    {

        
        public IndexModel(GradingContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly GradingContext _context;
        private readonly ILogger<IndexModel> _logger;


        public List<SchoolProgram> Programs { get; set; }
        public async Task OnGet()
        {
            Programs = await _context.Programs
                .Include(p => p.ProgramHead).ToListAsync();
        }

        public async Task<IActionResult> OnPostDelete([FromForm] int? programId)
        {
            _logger.LogInformation($"The program id is {programId}");
            if (programId == null)
            {
                return BadRequest();
            }
            SchoolProgram program = await _context.Programs.FindAsync(programId);
            if (program == null)
            {
                return NotFound();
            }
            _context.Programs.Remove(program);
            await _context.SaveChangesAsync();
            
            return StatusCode(200);

        }

        public class DeleteResult {
            public int StatusCode { get; set; }
            public bool IsDeleted { get; set; }
        }
    }
}
