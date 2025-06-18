using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels.Authen;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Service.Authen
{
    public class ManagerUserDAO
    {
        private readonly ClinicDbContext _context;

        public ManagerUserDAO(ClinicDbContext context)
        {
            _context = context;
        }

        // 1 + 3: Danh sách tài khoản có search + phân trang (6 account / trang)
        public List<Account> GetAccounts(string searchKeyword = "", int page = 1, int? roleId = null, bool? status = null)
        {
            var query = _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.User)
                .Include(a => a.Patient)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                query = query.Where(a =>
                    a.Username.Contains(searchKeyword) ||
                    (a.User != null && (
                        a.User.FullName.Contains(searchKeyword) ||
                        a.User.Email.Contains(searchKeyword)
                    )));
            }

            if (roleId.HasValue)
            {
                query = query.Where(a => a.RoleId == roleId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            return query
                .OrderBy(a => a.AccountId)
                .Skip((page - 1) * 6)
                .Take(6)
                .ToList();
        }
        // Lấy danh sách Role (async)
        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<int> CountAccountsAsync(string searchKeyword, int? roleId, bool? status)
        {
            var query = _context.Accounts
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(a =>
                    a.Username.Contains(searchKeyword) ||
                    (a.User != null && (
                        a.User.FullName.Contains(searchKeyword) ||
                        a.User.Email.Contains(searchKeyword))));
            }

            if (roleId.HasValue)
            {
                query = query.Where(a => a.RoleId == roleId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            return await query.CountAsync();
        }


        // 4: Tạo tài khoản mới (Status mặc định = false)
        public bool CreateAccount(UserAccountViewModel accountViewModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountViewModel.Username) || string.IsNullOrWhiteSpace(accountViewModel.Password))
                    throw new ArgumentException("Username và Password không được để trống.");

                var role = _context.Roles.Find(accountViewModel.RoleId);
                if (role == null)
                    throw new ArgumentException("Role không hợp lệ.");

                // Hash mật khẩu người dùng nhập
                string hashedPassword =(accountViewModel.Password);

                var account = new Account
                {
                    Username = accountViewModel.Username,
                    PasswordHash = hashedPassword,
                    Status = false, // mặc định khóa, hoặc true tùy bạn
                    RoleId = accountViewModel.RoleId
                };

                _context.Accounts.Add(account);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Console.WriteLine("CreateAccount error: " + ex.Message);
                return false;
            }
        }





        // 5: Xem chi tiết tài khoản theo ID
        public Account GetAccountById(int accountId)
        {
            return _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.User)
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.AccountId == accountId);
        }

        // 6: Xóa tài khoản nếu Status = false
        public bool DeleteAccount(int accountId)
        {
            try
            {
                var account = _context.Accounts.Find(accountId);
                if (account != null && account.Status == false)
                {
                    _context.Accounts.Remove(account);
                    _context.SaveChanges();
                    return true;
                }
                return false; // Không xóa nếu account đang hoạt động
            }
            catch
            {
                return false;
            }
        }

        // 7: Cập nhật tài khoản theo ID
        public bool UpdateAccount(UserAccountViewModel updatedAccount)
        {
            try
            {
                var existingAccount = _context.Accounts.Find(updatedAccount.AccountId);
                if (existingAccount != null)
                {
                    var role = _context.Roles.FirstOrDefault(r => r.RoleId == updatedAccount.RoleId);
                    if (role == null)
                        return false; // hoặc throw/lỗi nếu Role không tồn tại

                    existingAccount.Username = updatedAccount.Username;
                    existingAccount.RoleId = role.RoleId;

                    // Nếu muốn cập nhật FullName:
                    if (existingAccount.User != null)
                    {
                        existingAccount.User.FullName = updatedAccount.FullName;
                    }

                    // Nếu muốn cập nhật trạng thái:
                    existingAccount.Status = updatedAccount.Status;

                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

    }

}
