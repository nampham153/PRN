using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace QuanLyPhongKham.Pages.MedicinePage
{
     public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7086";
        }

        [BindProperty]
        public Medicine Medicine { get; set; } = new();

        [BindProperty]
        public int MedicineId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MedicineId = id;
            //if (id == null)
            //{
            //    return NotFound();
            //}

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

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            try
            {
                // Tạo ViewModel phù hợp với yêu cầu API
                var medicineVM = new MedicineVM
                {
                    MedicineName = Medicine.MedicineName,
                    Unit = Medicine.Unit,
                    Usage = Medicine.Usage
                };

                var apiUrl = $"{_apiBaseUrl}/api/Medicine/{Medicine.MedicineId}";
                var json = JsonSerializer.Serialize(medicineVM);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Medicine updated successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to update medicine: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating medicine: {ex.Message}";
            }

            return Page();
        }

    }
}