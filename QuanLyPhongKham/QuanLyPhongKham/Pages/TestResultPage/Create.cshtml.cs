using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace QuanLyPhongKham.Pages.TestResultPage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        public CreateModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7086";
        }

        [BindProperty]
        public TestResultVM TestResult { get; set; } = new TestResultVM();

        public List<SelectListItem> TestOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> MedicalRecordOptions { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get new TestResult template from API
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/new");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TestResult = JsonConvert.DeserializeObject<TestResultVM>(content) ?? new TestResultVM();
                }

                // Load dropdown options
                await LoadDropdownOptions();

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading create form: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownOptions();
                return Page();
            }

            try
            {
                var json = JsonConvert.SerializeObject(TestResult);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/TestResult", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Test result created successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorObj = JObject.Parse(errorContent);
                        if (errorObj["errors"] != null)
                        {
                            var errors = errorObj["errors"].ToObject<Dictionary<string, List<string>>>();
                            foreach (var error in errors)
                            {
                                foreach (var message in error.Value)
                                {
                                    ModelState.AddModelError(error.Key, message);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", errorObj["message"]?.ToString() ?? "Failed to create test result.");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Failed to create test result.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating test result: {ex.Message}");
            }

            await LoadDropdownOptions();
            return Page();
        }

        private async Task LoadDropdownOptions()
        {
            await Task.WhenAll(
                LoadTestOptions(),
                LoadUserOptions(),
                LoadMedicalRecordOptions()
            );
        }

        private async Task LoadTestOptions()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/Test");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Test API Response: {content}"); // Debug log

                    var jsonArray = JArray.Parse(content);
                    TestOptions = new List<SelectListItem>();

                    foreach (var item in jsonArray)
                    {
                        var testId = item["testId"]?.ToString() ?? item["TestId"]?.ToString() ?? "";
                        var testName = item["testName"]?.ToString() ?? item["TestName"]?.ToString() ?? "";
                        var description = item["description"]?.ToString() ?? item["Description"]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(testId) && !string.IsNullOrEmpty(testName))
                        {
                            var displayText = string.IsNullOrEmpty(description) ? testName : $"{testName} - {description}";
                            TestOptions.Add(new SelectListItem
                            {
                                Value = testId,
                                Text = displayText
                            });
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Test API Error: {response.StatusCode}");
                    TestOptions = new List<SelectListItem>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading test options: {ex.Message}");
                TestOptions = new List<SelectListItem>();
            }
        }

        private async Task LoadUserOptions()
        {
            try
            {
                // Sử dụng endpoint đã được verify từ IndexModel
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult/user-names");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"User API Response: {content}"); // Debug log

                    var userNames = JsonConvert.DeserializeObject<List<string>>(content) ?? new List<string>();
                    UserOptions = new List<SelectListItem>();

                    for (int i = 0; i < userNames.Count; i++)
                    {
                        UserOptions.Add(new SelectListItem
                        {
                            Value = (i + 1).ToString(), // Temporary ID - bạn cần thay đổi logic này
                            Text = userNames[i]
                        });
                    }

                    if (UserOptions.Any())
                    {
                        Console.WriteLine($"Successfully loaded {UserOptions.Count} users");
                    }
                }
                else
                {
                    Console.WriteLine($"User API Error: {response.StatusCode}");
                    // Fallback to sample data
                    UserOptions = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "Sample Technician 1" },
                        new SelectListItem { Value = "2", Text = "Sample Technician 2" }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user options: {ex.Message}");
                UserOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Default Technician" }
                };
            }
        }

        private async Task LoadMedicalRecordOptions()
        {
            try
            {
                // Sử dụng cùng logic như IndexModel để lấy dữ liệu TestResultVM
                // vì TestResultVM đã có đầy đủ thông tin PatientName
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/TestResult");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"TestResult API Response: {content}"); // Debug log

                    var testResults = JsonConvert.DeserializeObject<List<TestResultVM>>(content) ?? new List<TestResultVM>();

                    // Lấy các record duy nhất từ TestResult
                    var uniqueRecords = testResults
                        .Where(tr => tr.RecordId > 0) // Chỉ lấy record có ID hợp lệ
                        .GroupBy(tr => tr.RecordId)
                        .Select(g => g.First()) // Lấy record đầu tiên trong group
                        .ToList();

                    MedicalRecordOptions = new List<SelectListItem>();

                    foreach (var record in uniqueRecords)
                    {
                        // Tạo display text từ thông tin có sẵn trong TestResultVM
                        var displayText = CreateMedicalRecordDisplayText(record);

                        MedicalRecordOptions.Add(new SelectListItem
                        {
                            Value = record.RecordId.ToString(),
                            Text = displayText
                        });
                    }

                    // Sắp xếp theo tên bệnh nhân
                    MedicalRecordOptions = MedicalRecordOptions
                        .OrderBy(x => x.Text)
                        .ToList();

                    Console.WriteLine($"Successfully loaded {MedicalRecordOptions.Count} medical records");
                }
                else
                {
                    Console.WriteLine($"TestResult API Error: {response.StatusCode}");

                    // Fallback: thử gọi trực tiếp API MedicalRecord
                    await LoadMedicalRecordOptionsFallback();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical record options: {ex.Message}");

                // Fallback to sample data
                MedicalRecordOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Sample Patient 1 - Record #1" },
                    new SelectListItem { Value = "2", Text = "Sample Patient 2 - Record #2" },
                    new SelectListItem { Value = "3", Text = "Sample Patient 3 - Record #3" }
                };
            }
        }

        // Fallback method để load từ MedicalRecord API trực tiếp
        private async Task LoadMedicalRecordOptionsFallback()
        {
            try
            {
                string[] possibleEndpoints = {
                    $"{_apiBaseUrl}/api/MedicalRecord",
                    $"{_apiBaseUrl}/api/MedicalRecords",
                    $"{_apiBaseUrl}/api/Record",
                    $"{_apiBaseUrl}/api/Records"
                };

                foreach (string endpoint in possibleEndpoints)
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(endpoint);
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"MedicalRecord API Response from {endpoint}: {content}");

                            var jsonData = JToken.Parse(content);
                            MedicalRecordOptions = new List<SelectListItem>();

                            JArray recordArray;
                            if (jsonData is JArray directArray)
                            {
                                recordArray = directArray;
                            }
                            else if (jsonData is JObject obj)
                            {
                                recordArray = obj["data"]?.Value<JArray>() ??
                                            obj["records"]?.Value<JArray>() ??
                                            obj["medicalRecords"]?.Value<JArray>() ??
                                            obj["items"]?.Value<JArray>() ??
                                            new JArray();
                            }
                            else
                            {
                                recordArray = new JArray();
                            }

                            foreach (var item in recordArray)
                            {
                                var recordId = GetPropertyValue(item, new[] { "recordId", "RecordId", "id", "Id" });
                                if (string.IsNullOrEmpty(recordId)) continue;

                                var patientName = GetPatientName(item);
                                var createdDate = GetPropertyValue(item, new[] {
                                    "createdDate", "CreatedDate", "date", "Date",
                                    "visitDate", "VisitDate", "recordDate", "RecordDate"
                                });

                                var displayText = CreateDisplayText(patientName, recordId, createdDate);

                                MedicalRecordOptions.Add(new SelectListItem
                                {
                                    Value = recordId,
                                    Text = displayText
                                });
                            }

                            if (MedicalRecordOptions.Any())
                            {
                                Console.WriteLine($"Successfully loaded {MedicalRecordOptions.Count} medical records from {endpoint}");
                                MedicalRecordOptions = MedicalRecordOptions.OrderBy(x => x.Text).ToList();
                                return; // Thành công, thoát method
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error trying endpoint {endpoint}: {ex.Message}");
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in fallback method: {ex.Message}");
            }
        }

        // Tạo display text cho medical record từ TestResultVM
        private string CreateMedicalRecordDisplayText(TestResultVM testResult)
        {
            var displayText = "";

            // Ưu tiên hiển thị tên bệnh nhân nếu có
            if (!string.IsNullOrEmpty(testResult.PatientName))
            {
                displayText = testResult.PatientName;
            }
            else
            {
                displayText = $"Record #{testResult.RecordId}";
            }

            // Thêm thông tin ngày test nếu có
            if (testResult.TestDate != null && testResult.TestDate != default(DateTime))
            {
                displayText += $" - {testResult.TestDate:dd/MM/yyyy}";
            }

            // Thêm RecordId để phân biệt nếu có nhiều record cùng tên
            if (!string.IsNullOrEmpty(testResult.PatientName))
            {
                displayText += $" (ID: {testResult.RecordId})";
            }

            return displayText;
        }

        // Helper methods từ code cũ
        private string GetPropertyValue(JToken item, string[] possibleNames)
        {
            foreach (var name in possibleNames)
            {
                var value = item[name]?.ToString();
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return "";
        }

        private string GetPatientName(JToken item)
        {
            var directPatientName = GetPropertyValue(item, new[] {
                "patientName", "PatientName", "patientFullName", "PatientFullName",
                "patient", "Patient", "name", "Name", "fullName", "FullName"
            });

            if (!string.IsNullOrEmpty(directPatientName))
                return directPatientName;

            var patientObj = item["patient"] ?? item["Patient"];
            if (patientObj != null)
            {
                var nestedPatientName = GetPropertyValue(patientObj, new[] {
                    "name", "Name", "fullName", "FullName", "patientName", "PatientName",
                    "firstName", "FirstName", "lastName", "LastName"
                });

                if (!string.IsNullOrEmpty(nestedPatientName))
                    return nestedPatientName;

                var firstName = GetPropertyValue(patientObj, new[] { "firstName", "FirstName" });
                var lastName = GetPropertyValue(patientObj, new[] { "lastName", "LastName" });

                if (!string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
                {
                    return $"{firstName} {lastName}".Trim();
                }
            }

            var userObj = item["user"] ?? item["User"];
            if (userObj != null)
            {
                var userPatientName = GetPropertyValue(userObj, new[] {
                    "name", "Name", "fullName", "FullName", "userName", "UserName"
                });

                if (!string.IsNullOrEmpty(userPatientName))
                    return userPatientName;
            }

            return "";
        }

        private string CreateDisplayText(string patientName, string recordId, string createdDate)
        {
            var displayText = !string.IsNullOrEmpty(patientName) ? patientName : $"Record #{recordId}";

            if (!string.IsNullOrEmpty(createdDate))
            {
                if (DateTime.TryParse(createdDate, out DateTime date))
                {
                    displayText = $"{displayText} - {date:dd/MM/yyyy}";
                }
                else
                {
                    displayText = $"{displayText} - {createdDate}";
                }
            }

            displayText += $" (ID: {recordId})";
            return displayText;
        }
    }
}