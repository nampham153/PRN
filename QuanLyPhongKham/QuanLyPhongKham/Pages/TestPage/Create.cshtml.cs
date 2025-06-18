using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System.Text.Json;
using System.Text;

namespace Frontendui.Pages.TestPage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7086/api/Test";

        public CreateModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public TestVM Test { get; set; } = new TestVM();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Console.WriteLine($"Creating test: {Test.TestName}");

                var jsonString = JsonSerializer.Serialize(Test, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                Console.WriteLine($"JSON to send: {jsonString}");

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiBaseUrl, content);

                Console.WriteLine($"Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Test created successfully");
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error creating test: {errorContent}");
                    ModelState.AddModelError(string.Empty, "Error creating test: " + errorContent);
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating test: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
                return Page();
            }
        }
    }
}
