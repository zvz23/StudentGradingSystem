using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Pages.Admin.Program
{
    public class AddModel : PageModel
    {
        public AddModel(GradingContext context, ILogger<AddModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly GradingContext _context;
        private readonly ILogger<AddModel> _logger;

        [BindProperty]
        public InputModel ProgramHeadForm { get; set; }

        public List<SelectListItem> SelectProgramHead { get; set; } = new();

        public void OnGet()
        {
            foreach (var ph in _context.ProgramHeads.AsEnumerable())
            {
                SelectProgramHead.Add(new SelectListItem()
                {
                    Text = ph.Name,
                    Value = ph.ProgramHeadId.ToString()
                });
            }
            
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                SchoolProgram newProgram = new SchoolProgram();
                newProgram.ProgramName = ProgramHeadForm.ProgramName;

                if(ProgramHeadForm.ProgramHeadId != null)
                {
                    ProgramHead ph = await _context.ProgramHeads.FindAsync(ProgramHeadForm.ProgramHeadId);
                    if (ph == null)
                    {
                        ModelState.AddModelError("", "Program head not found");
                        return Page();
                    }
                    newProgram.ProgramHead = ph;
                }
                await _context.Programs.AddAsync(newProgram);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            return Page();
        }

        public class InputModel
        {
            [BindRequired]
            public string ProgramName { get; set; }
            public int? ProgramHeadId { get; set; }
        }
    }
}
