using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.IRepository.Authen
{
    public interface IAccountRepository
    {
        Account GetAccount(string username);
        void StoreRefreshToken(int accountId, string token, DateTime expiry);
        RefreshToken GetRefreshToken(string token);
    }

}
