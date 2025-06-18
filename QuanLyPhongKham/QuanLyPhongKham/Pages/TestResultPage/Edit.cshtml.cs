using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;

namespace QuanLyPhongKham.Pages.TestResultPage
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
        public TestResultVM TestResult { get; set; } = new TestResultVM();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public List<SelectListItem> MedicalRecords { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Tests { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Users { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;

            try
            {
                // Get test result data for editing
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/edit/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["ErrorMessage"] = "Test result not found.";
                        return RedirectToPage("./Index");
                    }

                    TempData["ErrorMessage"] = "Error loading test result data.";
                    return RedirectToPage("./Index");
                }

                var content = await response.Content.ReadAsStringAsync();
                TestResult = JsonConvert.DeserializeObject<TestResultVM>(content) ?? new TestResultVM();

                // Load dropdown data
                await LoadDropdownData();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading test result: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return Page();
            }

            try
            {
                var json = JsonConvert.SerializeObject(TestResult);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/TestResult/{Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Test result updated successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonConvert.DeserializeObject<dynamic>(errorContent);

                    if (errorObj?.errors != null)
                    {
                        var errors = JsonConvert.DeserializeObject<List<string>>(errorObj.errors.ToString());
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError(string.Empty, error);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, errorObj?.message?.ToString() ?? "Failed to update test result.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating test result: {ex.Message}");
            }

            await LoadDropdownData();
            return Page();
        }

        private async Task LoadDropdownData()
        {
            try
            {
                // Load Medical Records
                var medicalRecordsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/MedicalRecord");
                if (medicalRecordsResponse.IsSuccessStatusCode)
                {
                    var medicalRecordsContent = await medicalRecordsResponse.Content.ReadAsStringAsync();
                    var medicalRecords = JsonConvert.DeserializeObject<List<dynamic>>(medicalRecordsContent);

                    MedicalRecords = medicalRecords?.Select(mr => new SelectListItem
                    {
                        Value = mr.RecordId.ToString(),
                        Text = $"{mr.PatientName} - {mr.Date:dd/MM/yyyy} - {mr.Diagnosis}",
                        Selected = mr.RecordId == TestResult.RecordId
                    }).ToList() ?? new List<SelectListItem>();
                }

                // Load Tests
                var testsResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Test");
                if (testsResponse.IsSuccessStatusCode)
                {
                    var testsContent = await testsResponse.Content.ReadAsStringAsync();
                    var tests = JsonConvert.DeserializeObject<List<dynamic>>(testsContent);

                    Tests = tests?.Select(t => new SelectListItem
                    {
                        Value = t.TestId.ToString(),
                        Text = $"{t.TestName} - {t.Description}",
                        Selected = t.TestId == TestResult.TestId
                    }).ToList() ?? new List<SelectListItem>();
                }

                // Load Users (Technicians)
                var usersResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/User/technicians");
                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersContent = await usersResponse.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<dynamic>>(usersContent);

                    Users = users?.Select(u => new SelectListItem
                    {
                        Value = u.UserId.ToString(),
                        Text = $"{u.FullName} - {u.Role}",
                        Selected = u.UserId == TestResult.UserId
                    }).ToList() ?? new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                // If API calls fail, create empty lists to avoid errors
                MedicalRecords = new List<SelectListItem>();
                Tests = new List<SelectListItem>();
                Users = new List<SelectListItem>();

                TempData["ErrorMessage"] = $"Error loading dropdown data: {ex.Message}";
            }
        }
    }
}