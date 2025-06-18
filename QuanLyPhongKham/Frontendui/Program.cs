using DataAccessLayer.dbcontext;
using Microsoft.EntityFrameworkCore;

namespace Frontendui
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ClinicDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient();

            // Hoặc cấu hình HttpClient với base address cụ thể
            builder.Services.AddHttpClient("TestApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7086/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
