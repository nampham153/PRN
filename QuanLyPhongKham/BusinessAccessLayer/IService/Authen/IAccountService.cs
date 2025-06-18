using DataAccessLayer.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.IService.Authen
{
    public interface IAccountService
    {
        string Login(string username, string password);
        RefreshToken GetRefreshToken(string token);

        string GenerateJwtTokenFromRefreshToken(string refreshToken);
    }

}
