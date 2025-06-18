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
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7086/api/Test";

        public IndexModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IList<TestVM> Test { get; set; } = new List<TestVM>();

        public async Task OnGetAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Test = JsonSerializer.Deserialize<List<TestVM>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<TestVM>();
                }
                else
                {
                    // Log error or handle failure
                    Test = new List<TestVM>();
                }
            }
            catch (Exception ex)
            {
                // Log exception
                Test = new List<TestVM>();
            }
        }
    }
}
