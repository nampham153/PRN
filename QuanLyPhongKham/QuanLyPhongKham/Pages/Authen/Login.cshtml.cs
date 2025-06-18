using DataAccessLayer.ViewModels.Authen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace QuanLyPhongKham.Pages.Authen
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginRequest LoginRequest { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7086/api/auth/login", LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonDocument.Parse(responseBody);
                var token = json.RootElement.GetProperty("token").GetString();

                // Lưu token vào session hoặc cookie
                HttpContext.Session.SetString("JWTToken", token);

                // Chuyển hướng sau khi đăng nhập thành công
                return RedirectToPage("/Authen/Index");
            }

            ErrorMessage = "Invalid username or password";
            return Page();
        }
    }
}
