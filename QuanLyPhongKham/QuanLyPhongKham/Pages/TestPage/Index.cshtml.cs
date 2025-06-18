using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System.Text.Json;
using DataAccessLayer.ViewModels.Search;
using System.Text;

namespace Frontendui.Pages.TestPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7086/api/Test";

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Properties for search and filter
        [BindProperty(SupportsGet = true)]
        public SearchFilterVM SearchFilter { get; set; } = new SearchFilterVM();

        public PaginatedResult<Test> PaginatedResult { get; set; }

        // Keep this for backward compatibility if needed
        public IList<Test> Test { get; set; } = new List<Test>();

        public async Task OnGetAsync()
        {
            try
            {
                // Validate and set defaults for SearchFilter
                if (SearchFilter == null)
                {
                    SearchFilter = new SearchFilterVM();
                }

                // Set defaults if not provided
                if (SearchFilter.PageNumber <= 0)
                    SearchFilter.PageNumber = 1;

                if (SearchFilter.PageSize <= 0)
                    SearchFilter.PageSize = 10;

                if (string.IsNullOrEmpty(SearchFilter.SortBy))
                    SearchFilter.SortBy = "TestId";

                // Build query parameters
                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(SearchFilter.SearchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(SearchFilter.SearchTerm)}");

                queryParams.Add($"sortBy={Uri.EscapeDataString(SearchFilter.SortBy)}");
                queryParams.Add($"sortDescending={SearchFilter.SortDescending.ToString().ToLower()}");
                queryParams.Add($"pageNumber={SearchFilter.PageNumber}");
                queryParams.Add($"pageSize={SearchFilter.PageSize}");

                var queryString = string.Join("&", queryParams);
                var apiUrl = $"{_apiBaseUrl}/filter?{queryString}";

                Console.WriteLine($"Calling API: {apiUrl}");
                var response = await _httpClient.GetAsync(apiUrl);
                Console.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonString}");

                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        PaginatedResult = JsonSerializer.Deserialize<PaginatedResult<Test>>(jsonString, options);

                        if (PaginatedResult != null)
                        {
                            Console.WriteLine($"Deserialized {PaginatedResult.Data?.Count ?? 0} items, Total: {PaginatedResult.TotalRecords}");

                            // Set the Test property for backward compatibility
                            Test = PaginatedResult.Data ?? new List<Test>();
                        }
                        else
                        {
                            Console.WriteLine("Failed to deserialize paginated result");
                            PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter.PageNumber, SearchFilter.PageSize);
                            Test = new List<Test>();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Response is empty");
                        PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter.PageNumber, SearchFilter.PageSize);
                        Test = new List<Test>();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API call failed with status: {response.StatusCode}");
                    Console.WriteLine($"Error content: {errorContent}");

                    PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter.PageNumber, SearchFilter.PageSize);
                    Test = new List<Test>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Exception: {httpEx.Message}");
                PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter?.PageNumber ?? 1, SearchFilter?.PageSize ?? 10);
                Test = new List<Test>();
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Exception: {jsonEx.Message}");
                PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter?.PageNumber ?? 1, SearchFilter?.PageSize ?? 10);
                Test = new List<Test>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter?.PageNumber ?? 1, SearchFilter?.PageSize ?? 10);
                Test = new List<Test>();
            }
        }

        // Method for advanced search using POST (optional)
        public async Task<IActionResult> OnPostAdvancedSearchAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(SearchFilter, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/advanced-search", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    PaginatedResult = JsonSerializer.Deserialize<PaginatedResult<Test>>(jsonString, options);
                    Test = PaginatedResult?.Data ?? new List<Test>();
                }
                else
                {
                    PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter.PageNumber, SearchFilter.PageSize);
                    Test = new List<Test>();
                }

                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Advanced Search Exception: {ex.Message}");
                PaginatedResult = new PaginatedResult<Test>(new List<Test>(), 0, SearchFilter.PageNumber, SearchFilter.PageSize);
                Test = new List<Test>();
                return Page();
            }
        }
    }
}