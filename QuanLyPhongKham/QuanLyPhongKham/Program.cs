using BusinessAccessLayer.IService;
using BusinessAccessLayer.IService.Authen;
using BusinessAccessLayer.Service;
using BusinessAccessLayer.Service.Authen;
using DataAccessLayer.DAO;
using DataAccessLayer.DAO.Authen;
using DataAccessLayer.dbcontext;
using DataAccessLayer.IRepository;
using DataAccessLayer.IRepository.Authen;
using DataAccessLayer.models;
using DataAccessLayer.Repository;
using DataAccessLayer.Repository.Authen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;

namespace QuanLyPhongKham
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========================================
            // Register DbContext
            // ========================================
            builder.Services.AddDbContext<ClinicDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"));
            });

            // ========================================
            // Register Services and Repositories
            // ========================================

            // Patient-related services
            builder.Services.AddScoped<MedicineDAO>();
            builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<PatientDAO>();
            builder.Services.AddScoped<IPatientRepository, PatientRepository>();
            builder.Services.AddScoped<IPatientService, PatientService>();

            // General Test Logic
            builder.Services.AddScoped<TestDAO>();
            builder.Services.AddScoped<ITestRepository, TestRepository>();
            builder.Services.AddScoped<ITestService, TestService>();

            builder.Services.AddScoped<ITestService, TestService>();
            builder.Services.AddScoped<ITestRepository, TestRepository>();
            builder.Services.AddScoped<DoctorDAO>();
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
            builder.Services.AddScoped<DataAccessLayer.IRepository.IAccountRepository, DataAccessLayer.Repository.AccountRepository>();

            builder.Services.AddScoped<TestResultDAO>();
            builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
            builder.Services.AddScoped<ITestResultService, TestResultService>();


            // ManagerUser Logic
            builder.Services.AddScoped<ManagerUserDAO>();
            builder.Services.AddScoped<IManagerUserRepository, ManagerUserRepository>();
            builder.Services.AddScoped<IManagerUserService, ManagerUserService>();

            // Account Logic
            builder.Services.AddScoped<AccountDAO>();
            builder.Services.AddScoped<DataAccessLayer.Repository.Authen.AccountRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddControllers()
                .AddOData(opt =>
                {
                    opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                       .AddRouteComponents("odata", GetEdmModel());
                });


            // ========================================
            // Register MVC & JSON options
            // ========================================
            builder.Services.AddControllers();

            // Đọc cấu hình JWT từ appsettings.json
            var jwtSecret = builder.Configuration["Jwt:Key"];
            var jwtIssuer = builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? jwtIssuer; // Nếu không có Audience thì lấy Issuer

            // Đăng ký Authentication với JWT Bearer
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            IEdmModel GetEdmModel()
            {
                var odataBuilder = new ODataConventionModelBuilder();
                odataBuilder.EntitySet<TestResult>("TestResults");
                odataBuilder.EntitySet<User>("Doctors");
                return odataBuilder.GetEdmModel();
            }

            // ========================================
            // Build & Configure Middleware
            // ========================================
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Rất quan trọng để hiển thị lỗi
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
         Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                RequestPath = ""
            });

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }

    }
}