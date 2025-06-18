using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;

namespace Frontendui.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataAccessLayer.dbcontext.ClinicDbContext _context;

        public IndexModel(DataAccessLayer.dbcontext.ClinicDbContext context)
        {
            _context = context;
        }

        public IList<Account> Account { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Account = await _context.Accounts
                .Include(a => a.Role).ToListAsync();
        }
    }
}
