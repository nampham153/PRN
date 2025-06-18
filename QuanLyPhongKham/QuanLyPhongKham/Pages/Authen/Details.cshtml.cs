using DataAccessLayer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace QuanLyPhongKham.Pages.Authen
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailsModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        public Account Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7086/api/account/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            Account = await response.Content.ReadFromJsonAsync<Account>();
            return Page();
        }
    }
}
