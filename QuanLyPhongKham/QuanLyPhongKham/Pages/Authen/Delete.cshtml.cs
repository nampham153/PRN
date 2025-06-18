using DataAccessLayer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace QuanLyPhongKham.Pages.Authen
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [BindProperty]
        public Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7086/api/account/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Account = await response.Content.ReadFromJsonAsync<Account>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7086/api/account/{Account.AccountId}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Xóa tài khoản thất bại");
                return Page();
            }

            return RedirectToPage("/Authen/Index");
        }
    }
}
