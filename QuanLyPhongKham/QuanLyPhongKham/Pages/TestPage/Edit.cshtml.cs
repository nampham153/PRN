using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.dbcontext;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using System.Text.Json;
using System.Text;

namespace Frontendui.Pages.TestPage
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7086/api/Test";

        public EditModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [BindProperty]
        public Test Test { get; set; } = new Test();

        [BindProperty]
        public int TestId { get; set; } // Thêm property này để bind với hidden input

        public async Task<IActionResult> OnGetAsync(int id)
        {
            TestId = id; // Set TestId

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Test = JsonSerializer.Deserialize<Test>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new Test();

                    return Page();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Bỏ ModelState validation tạm thời để test
            // if (!ModelState.IsValid)
            // {
            //     return Page();
            // }

            try
            {
                // Sử dụng TestId từ hidden input thay vì Test.TestId
                var testVM = new TestVM
                {
                    TestName = Test.TestName ?? "",
                    Description = Test.Description ?? ""
                };

                var jsonString = JsonSerializer.Serialize(testVM, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/{TestId}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Force redirect
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error: {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception: {ex.Message}");
                return Page();
            }
        }
    }
}