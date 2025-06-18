using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using DataAccessLayer.models;

namespace QuanLyPhongKham.Pages.TestResultPage
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

        public List<TestResultVM> TestResults { get; set; } = new List<TestResultVM>();
        public List<SelectListItem> TestNameOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserNameOptions { get; set; } = new List<SelectListItem>();
        public List<TestResult> TestResultss { get; set; } = new List<TestResult>();

        // Thêm Dictionary để map RecordId với ResultId
        public Dictionary<int, int> ResultIdMapping { get; set; } = new Dictionary<int, int>();

        [BindProperty(SupportsGet = true)]
        public string? SelectedTestName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedUserName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Load filter options
                await LoadTestNameOptions();
                await LoadUserNameOptions();

                // Build API URL with filter parameters
                var apiUrl = $"{_apiBaseUrl}/api/TestResult";
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(SelectedTestName))
                {
                    queryParams.Add($"testName={Uri.EscapeDataString(SelectedTestName)}");
                }

                if (!string.IsNullOrEmpty(SelectedUserName))
                {
                    queryParams.Add($"userName={Uri.EscapeDataString(SelectedUserName)}");
                }

                if (queryParams.Any())
                {
                    apiUrl += "?" + string.Join("&", queryParams);
                }

                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TestResults = JsonConvert.DeserializeObject<List<TestResultVM>>(content) ?? new List<TestResultVM>();

                    // Load thêm dữ liệu TestResult để lấy ResultId mapping
                    await LoadResultIdMapping();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to load test results. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading test results: {ex.Message}";
            }
            return Page();
        }

        private async Task LoadResultIdMapping()
        {
            try
            {
                // Gọi API endpoint mới để lấy danh sách TestResult với ResultId
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/with-resultid");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var testResults = JsonConvert.DeserializeObject<List<TestResult>>(content) ?? new List<TestResult>();

                    // Tạo mapping giữa RecordId và ResultId
                    // Chú ý: Một RecordId có thể có nhiều TestResult, nhưng ở đây lấy ResultId đầu tiên
                    // Nếu cần lấy theo logic khác (như ResultId lớn nhất), có thể điều chỉnh
                    ResultIdMapping = testResults
                        .GroupBy(tr => tr.RecordId)
                        .ToDictionary(g => g.Key, g => g.First().ResultId);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed, but don't break the page load
                ResultIdMapping = new Dictionary<int, int>();
            }
        }

        private async Task LoadTestNameOptions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/test-names");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var testNames = JsonConvert.DeserializeObject<List<string>>(content) ?? new List<string>();

                    TestNameOptions = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "All Test Types", Selected = string.IsNullOrEmpty(SelectedTestName) }
                    };

                    TestNameOptions.AddRange(testNames.Select(name => new SelectListItem
                    {
                        Value = name,
                        Text = name,
                        Selected = name == SelectedTestName
                    }));
                }
            }
            catch (Exception ex)
            {
                // Log error if needed, but don't break the page load
                TestNameOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "All Test Types" }
                };
            }
        }

        private async Task LoadUserNameOptions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/user-names");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var userNames = JsonConvert.DeserializeObject<List<string>>(content) ?? new List<string>();

                    UserNameOptions = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "All Technicians", Selected = string.IsNullOrEmpty(SelectedUserName) }
                    };

                    UserNameOptions.AddRange(userNames.Select(name => new SelectListItem
                    {
                        Value = name,
                        Text = name,
                        Selected = name == SelectedUserName
                    }));
                }
            }
            catch (Exception ex)
            {
                // Log error if needed, but don't break the page load
                UserNameOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "All Technicians" }
                };
            }
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
                    return RedirectToPage();
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
                    try
                    {
                        var errorObj = JObject.Parse(errorContent);
                        TempData["ErrorMessage"] = errorObj["message"]?.ToString() ?? "Failed to delete test result.";
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "Failed to delete test result.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting test result: {ex.Message}";
            }

            // Preserve filters when redirecting after delete
            var routeValues = new RouteValueDictionary();
            if (!string.IsNullOrEmpty(SelectedTestName))
                routeValues.Add("selectedTestName", SelectedTestName);
            if (!string.IsNullOrEmpty(SelectedUserName))
                routeValues.Add("selectedUserName", SelectedUserName);

            return RedirectToPage(routeValues);
        }

        // Helper method để lấy ResultId từ RecordId
        public int GetResultId(int recordId)
        {
            return ResultIdMapping.TryGetValue(recordId, out int resultId) ? resultId : 0;
        }
    }
}