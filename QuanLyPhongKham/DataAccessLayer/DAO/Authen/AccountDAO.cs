using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DAO.Authen
{
    public class AccountDAO
    {
        private readonly ClinicDbContext _context;

        public AccountDAO(ClinicDbContext context)
        {
            _context = context;
        }

        public Account GetByUsername(string username)
        {
            return _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.RefreshToken)
                .FirstOrDefault(a => a.Username == username);
        }

        //public Account GetByUsername(string username)
        //{
        //    var account = _context.Accounts
        //        .Include(a => a.Role)
        //        .Include(a => a.RefreshToken)
        //        .FirstOrDefault(a => a.Username == username);

        //    // Token đã có sẵn trong account.RefreshToken.Token
        //    if (account?.RefreshToken != null)
        //    {
        //        var token = account.RefreshToken.Token; // Đây là Token bạn cần
        //        Console.WriteLine($"Token: {token}");
        //    }

        //    return account;
        //}
        public void SaveRefreshToken(int accountId, string token, DateTime expiryDate)
        {
            var existing = _context.RefreshTokens.FirstOrDefault(r => r.AccountId == accountId);
            if (existing != null)
            {
                existing.Token = token;
                existing.ExpiryDate = expiryDate;
                existing.CreatedDate = DateTime.UtcNow;
            }
            else
            {
                _context.RefreshTokens.Add(new RefreshToken
                {
                    AccountId = accountId,
                    Token = token,
                    ExpiryDate = expiryDate,
                    CreatedDate = DateTime.UtcNow
                });
            }

            _context.SaveChanges();
        }

        public RefreshToken GetRefreshToken(string token)
        {
            return _context.RefreshTokens
                .Include(r => r.Account).ThenInclude(a => a.Role)
                .FirstOrDefault(r => r.Token == token);
        }


    }

}
