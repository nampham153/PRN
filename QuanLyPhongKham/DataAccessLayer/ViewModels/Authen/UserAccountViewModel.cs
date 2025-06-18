using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels.Authen
{
    public class UserAccountViewModel
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }       // ✅ Thêm thuộc tính này để binding với dropdown
        public string FullName { get; set; }
        public bool Status { get; set; } // true = active, false = locked
        public string Password { get; set; }  // thêm trường mật khẩu nhập từ form

    }

}
