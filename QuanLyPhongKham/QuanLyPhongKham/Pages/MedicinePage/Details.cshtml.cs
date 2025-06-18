using DataAccessLayer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace QuanLyPhongKham.Pages.MedicinePage
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public DetailsModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7086";
        }

        public Medicine Medicine { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var apiUrl = $"{_apiBaseUrl}/api/Medicine/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Medicine = JsonSerializer.Deserialize<Medicine>(jsonContent, options);

                    if (Medicine == null)
                    {
                        return NotFound();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load medicine from server.";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading medicine: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return Page();
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

            return RedirectToPage("./Index");
        }
    }
}