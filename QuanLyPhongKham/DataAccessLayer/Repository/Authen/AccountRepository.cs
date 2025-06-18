using DataAccessLayer.DAO.Authen;
using DataAccessLayer.IRepository.Authen;
using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.Authen
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepository(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        public Account GetAccount(string username)
        {
            return _accountDAO.GetByUsername(username);
        }

        public void StoreRefreshToken(int accountId, string token, DateTime expiry)
        {
            _accountDAO.SaveRefreshToken(accountId, token, expiry);
        }

        public RefreshToken GetRefreshToken(string token)
        {
            return _accountDAO.GetRefreshToken(token);
        }
    }

}
