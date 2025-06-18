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

namespace Frontendui.Pages.TestPage
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7086/api/Test";

        public DetailsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Test Test { get; set; } = new Test();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Console.WriteLine($"Getting test details for ID: {id}");

                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                Console.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonString}");

                    Test = JsonSerializer.Deserialize<Test>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new Test();

                    Console.WriteLine($"Loaded test: {Test.TestName}");
                    return Page();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error getting test: {errorContent}");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception getting test: {ex.Message}");
                return NotFound();
            }
        }
    }
}
