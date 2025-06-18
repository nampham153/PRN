using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.models;

namespace DataAccessLayer.IRepository
{
    public interface IAccountRepository
    {
        Account GetAccountById(int id);
        List<Account> GetDoctorAccounts();
        List<Account> GetAvailableAccountsForDoctor();
    }
}
