// DataAccessLayer/DAO/DoctorDAO.cs
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.DAO
{
    public class DoctorDAO
    {
        private readonly ClinicDbContext _context;

        public DoctorDAO(ClinicDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetAllDoctors()
        {
            return _context.Users.Include(u => u.Account).Where(u => u.Account.RoleId == 2);
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


        public User? GetByAccountId(int accountId)
        {
            return _context.Users.Include(u => u.Account)
                                 .FirstOrDefault(u => u.AccountId == accountId && u.Account.RoleId == 2);
        }

        public bool ExistsUserByAccountId(int accountId)
        {
            return _context.Users.Any(u => u.AccountId == accountId);
        }

        public Account? GetAccount(int accountId)
        {
            return _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
        }

        public void Add(User doctor)
        {
            _context.Users.Add(doctor);
            _context.SaveChanges();
        }

        public void Update(User doctor)
        {
            _context.Users.Update(doctor);
            _context.SaveChanges();
        }

        public void Delete(User doctor)
        {
            _context.Users.Remove(doctor);
            _context.SaveChanges();
        }
    }
}

