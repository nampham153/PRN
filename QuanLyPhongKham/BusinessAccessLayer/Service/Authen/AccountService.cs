using BusinessAccessLayer.IService.Authen;
using DataAccessLayer.models;
using DataAccessLayer.Repository.Authen;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessAccessLayer.Service.Authen
{
    public class AccountService : IAccountService
    {
        private readonly AccountRepository _accountRepository;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly IConfiguration _configuration;

        public AccountService(AccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
            // Lấy từ appsettings.json
            _jwtSecret = configuration["Jwt:Key"];
            _jwtIssuer = configuration["Jwt:Issuer"];
        }

        public string Login(string username, string password)
        {
            var account = _accountRepository.GetAccount(username);
            if (account == null)
                return null;

            // TODO: Nên hash password và so sánh hash thay vì plain text
            if (account.PasswordHash != password)
                return null;

            var jwtToken = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken();
            
            _accountRepository.StoreRefreshToken(account.AccountId, refreshToken.Token, refreshToken.ExpiryDate);
            
            return jwtToken;
        }

        private string GenerateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Cách 1: Tạo key trực tiếp
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSecret);
            var signingKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Name, account.Username ?? ""),
                new Claim("role", account.Role?.ToString() ?? "User"), // Sử dụng ToString() để đảm bảo string
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            // Tạo token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "15")),
                Issuer = _jwtIssuer,
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = signingCredentials
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedDate = DateTime.UtcNow
            };
        }

        public RefreshToken GetRefreshToken(string token)
        {
            return _accountRepository.GetRefreshToken(token);
        }

        public string GenerateJwtTokenFromRefreshToken(string refreshToken)
        {
            // Debug: Log refresh token
            Console.WriteLine($"Received refresh token: {refreshToken}");
            
            // 1. Lấy RefreshToken từ repository
            var storedToken = _accountRepository.GetRefreshToken(refreshToken);
            if (storedToken == null)
            {
                Console.WriteLine("Refresh token not found in database");
                return null; // Token không tồn tại
            }
                
            if (storedToken.ExpiryDate <= DateTime.UtcNow)
            {
                Console.WriteLine($"Refresh token expired. Expiry: {storedToken.ExpiryDate}, Now: {DateTime.UtcNow}");
                return null; // Token hết hạn
            }

            // 2. Lấy Account liên quan
            var account = storedToken.Account;
            if (account == null)
            {
                Console.WriteLine("Account not found for refresh token");
                return null;
            }

            Console.WriteLine($"Generating new JWT for user: {account.Username}");

            // 3. Tạo JWT token mới dựa trên Account
            var newJwtToken = GenerateJwtToken(account);

            // 4. (Tuỳ chọn) Cập nhật refresh token mới hoặc giữ nguyên
            return newJwtToken;
        }
    }
}