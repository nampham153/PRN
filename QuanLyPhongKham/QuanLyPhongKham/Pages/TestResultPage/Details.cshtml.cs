using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace QuanLyPhongKham.Pages.TestResultPage
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
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7000";
        }

        public TestResultVM TestResult { get; set; } = new TestResultVM();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;

            try
            {
                // Get test result details
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/edit/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = "Test result not found.";
                        return RedirectToPage("./Index");
                    }

                    TempData["ErrorMessage"] = "Error loading test result details.";
                    return RedirectToPage("./Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                TestResult = JsonConvert.DeserializeObject<TestResultVM>(content) ?? new TestResultVM();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading test result details: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                // First check if test result exists
                var existsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/exists/{id}");
                if (!existsResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Test result not found.";
                    return RedirectToPage("./Index");
                }

                // Delete the test result
                var deleteResponse = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/TestResult/{id}");

                if (deleteResponse.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Test result deleted successfully.";
                }
                else
                {
                    var errorContent = await deleteResponse.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);
                    TempData["ErrorMessage"] = errorObj?.message?.ToString() ?? "Failed to delete test result.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting test result: {ex.Message}";
            }

            return RedirectToPage("./Index");
        }
    }
}