using DataAccessLayer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace QuanLyPhongKham.Pages.Authen
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(IHttpClientFactory factory, IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? RoleId { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;


        public List<Account> Accounts { get; set; } = new();  // Dùng Account entity
        public List<Role> Roles { get; set; } = new();
        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            Console.WriteLine($"Page nhận được: {Page}");

            // Lấy danh sách Roles
            var rolesResponse = await _httpClient.GetAsync("https://localhost:7086/api/account/roles");
            if (rolesResponse.IsSuccessStatusCode)
            {
                var rolesJson = await rolesResponse.Content.ReadAsStringAsync();
                Roles = JsonSerializer.Deserialize<List<Role>>(rolesJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }

            // Xây dựng query string
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(Search))
                queryParams.Add($"searchKeyword={Uri.EscapeDataString(Search)}");

            if (RoleId.HasValue)
                queryParams.Add($"roleId={RoleId.Value}");

            if (Status.HasValue)
                queryParams.Add($"status={Status.Value}");

            var countQuery = string.Join("&", queryParams);
            var countUrl = $"https://localhost:7086/api/account/count";
            if (!string.IsNullOrEmpty(countQuery))
                countUrl += "?" + countQuery;

            var countResponse = await _httpClient.GetAsync(countUrl);
            if (countResponse.IsSuccessStatusCode)
            {
                var countJson = await countResponse.Content.ReadAsStringAsync();
                if (int.TryParse(countJson, out int totalCount))
                {
                    TotalPages = (int)Math.Ceiling(totalCount / 6.0);
                }
            }

            // Validate page số hợp lệ
            if (Page < 1) Page = 1;
            if (Page > TotalPages) Page = TotalPages;

            queryParams.Add($"page={Page}");
            var listQuery = string.Join("&", queryParams);
            var listUrl = $"https://localhost:7086/api/account/list";
            if (!string.IsNullOrEmpty(listQuery))
                listUrl += "?" + listQuery;

            var accResponse = await _httpClient.GetAsync(listUrl);
            if (accResponse.IsSuccessStatusCode)
            {
                var accJson = await accResponse.Content.ReadAsStringAsync();
                Accounts = JsonSerializer.Deserialize<List<Account>>(accJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }
        }


    }
}
