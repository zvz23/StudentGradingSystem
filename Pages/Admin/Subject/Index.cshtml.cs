using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using backend.Data;

namespace backend.Pages.Admin.Subject
{
    public class IndexModel : PageModel
    {
        public IndexModel(GradingContext context, ILogger<IndexModel> logger) {
            _context = context;
            _logger = logger;
        }

        private readonly GradingContext _context;
        private readonly ILogger<IndexModel> _logger;

        public List<backend.Models.Subject> Subjects { get; set; } = new();
        public async Task OnGet()
        {
            Subjects = await _context.Subjects
            .Include(s => s.SchoolProgram)
            .Include(s => s.Prerequisite)
            .ThenInclude(ps => ps.Subjects)
            .ToListAsync();
        }
    }
}
