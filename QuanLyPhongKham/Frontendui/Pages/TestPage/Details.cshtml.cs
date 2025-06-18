using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;

namespace Frontendui.Pages.TestPage
{
    public class DetailsModel : PageModel
    {
        private readonly DataAccessLayer.dbcontext.ClinicDbContext _context;

        public DetailsModel(DataAccessLayer.dbcontext.ClinicDbContext context)
        {
            _context = context;
        }

        public Test Test { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FirstOrDefaultAsync(m => m.TestId == id);
            if (test == null)
            {
                return NotFound();
            }
            else
            {
                Test = test;
            }
            return Page();
        }
    }
}
