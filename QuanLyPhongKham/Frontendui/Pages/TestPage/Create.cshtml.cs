using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;

namespace Frontendui.Pages.TestPage
{
    public class CreateModel : PageModel
    {
        private readonly DataAccessLayer.dbcontext.ClinicDbContext _context;

        public CreateModel(DataAccessLayer.dbcontext.ClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Test Test { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Tests.Add(Test);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
