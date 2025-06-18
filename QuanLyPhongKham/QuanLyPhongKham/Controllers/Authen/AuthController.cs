using BusinessAccessLayer.IService.Authen;
using DataAccessLayer.ViewModels.Authen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace QuanLyPhongKham.Controllers.Authen
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] DataAccessLayer.ViewModels.Authen.LoginRequest model)
        {
            var token = _accountService.Login(model.Username, model.Password);
            // Debug: Log token để kiểm tra
            Console.WriteLine($"Generated token: {token}");

            if (token == null)
                return Unauthorized(new { message = "Username or password is incorrect" });
            return Ok(new { token });
        }

        // POST api/auth/refresh-token
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var refreshToken = _accountService.GetRefreshToken(model.Token);
            if (refreshToken == null || refreshToken.ExpiryDate < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            var newJwt = _accountService.GenerateJwtTokenFromRefreshToken(refreshToken.Token);  // <-- sửa chỗ này

            return Ok(new { token = newJwt });
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            // Bạn chỉ có thể trả status code hoặc JSON, không redirect
            return Ok(new { message = "Bạn đã đến được API AuthController - Index" });
        }
    }
}
