using BusinessAccessLayer.IService.Authen;
using DataAccessLayer.IRepository.Authen;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels.Authen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Service.Authen
{
    public class ManagerUserService : IManagerUserService
    {
        private readonly IManagerUserRepository _repository;

        public ManagerUserService(IManagerUserRepository repository)
        {
            _repository = repository;
        }


      

        public List<Account> GetAccounts(string searchKeyword, int page, int? roleId, bool? status)
        {
            if (page < 1) page = 1;
            return _repository.GetAccounts(searchKeyword, page, roleId, status);
        }


        public bool CreateAccount(UserAccountViewModel account)
        {
            // Business rule: kiểm tra dữ liệu hợp lệ nếu cần
            if (string.IsNullOrWhiteSpace(account.Username))
                return false;

            return _repository.CreateAccount(account);
        }

        public Account GetAccountById(int accountId)
        {
            return _repository.GetAccountById(accountId);
        }

        public bool DeleteAccount(int accountId)
        {
            return _repository.DeleteAccount(accountId);
        }

        public bool UpdateAccount(UserAccountViewModel updatedAccount)
        {
            return _repository.UpdateAccount(updatedAccount);
        }

        public async Task<int> CountAccountsAsync(string searchKeyword, int? roleId, bool? status)
        {
            return await _repository.CountAccountsAsync(searchKeyword, roleId, status);
        }
        public async Task<List<Role>> GetRolesAsync()
        {
            // Nếu cần xử lý logic gì thêm thì thêm ở đây
            return await _repository.GetRolesAsync();
        }
    }

}
