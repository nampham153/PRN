using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.dbcontext;
using DataAccessLayer.IRepository;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DataAccessLayer.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ClinicDbContext _context;

        public AccountRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public Account GetAccountById(int accountid)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountid);
        }
        public List<Account> GetDoctorAccounts()
        {
            return _context.Accounts
                .Where(a => a.RoleId == 2)
                .ToList();
        }
        public List<Account> GetAvailableAccountsForDoctor()
        {
            var usedAccountIds = _context.Users
                .Where(u => u.Account.RoleId == 2)
                .Select(u => u.AccountId)
                .ToList();

            return _context.Accounts
                .Where(a => a.RoleId == 2 && !usedAccountIds.Contains(a.AccountId))
                .ToList();
        }

    }
}
