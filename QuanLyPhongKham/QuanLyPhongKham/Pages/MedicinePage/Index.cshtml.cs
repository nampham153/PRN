using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace QuanLyPhongKham.Pages.MedicinePage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7086";
        }

        public List<DataAccessLayer.models.Medicine> Medicines { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var apiUrl = $"{_apiBaseUrl}/api/Medicine";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Medicines = JsonSerializer.Deserialize<List<DataAccessLayer.models.Medicine>>(jsonContent, options) ?? new();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load medicines from server.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading medicines: {ex.Message}";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var apiUrl = $"{_apiBaseUrl}/api/Medicine/{id}";
                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Medicine deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete medicine.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting medicine: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}