using BusinessAccessLayer.IService;
using DataAccessLayer.IRepository;
using DataAccessLayer.ViewModels;
using DataAccessLayer.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyPhongKham.Pages.Doctors
{
    public class CreateModel : PageModel
    {
        private readonly IDoctorService _doctorService;
        private readonly IAccountRepository _accountRepository;

        public CreateModel(IDoctorService doctorService, IAccountRepository accountRepository)
        {
            _doctorService = doctorService;
            _accountRepository = accountRepository;
        }

        [BindProperty]
        public DoctorVM Doctor { get; set; }

        public List<Account> DoctorAccounts { get; set; }

        public void OnGet()
        {
            Doctor = new DoctorVM();
            DoctorAccounts = _accountRepository.GetAvailableAccountsForDoctor();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            DoctorAccounts = _accountRepository.GetAvailableAccountsForDoctor();

            if ((Doctor.DoctorFile == null || Doctor.DoctorFile.Length == 0) && string.IsNullOrWhiteSpace(Doctor.DoctorPath))
            {
                ModelState.AddModelError("Doctor.DoctorFile", "Bạn phải chọn ảnh hoặc nhập URL.");
            }

            if (!ModelState.IsValid)
                return Page();

            if (!Doctor.DOB.HasValue)
            {
                ModelState.AddModelError("Doctor.DOB", "Vui lòng nhập ngày sinh.");
                return Page();
            }

            var dob = Doctor.DOB.Value;
            var age = DateTime.Today.Year - dob.Year;
            if (dob > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
            {
                ModelState.AddModelError("Doctor.DOB", "Bác sĩ phải từ 18 tuổi trở lên.");
                return Page();
            }

            var account = _accountRepository.GetAccountById(Doctor.AccountId);
            if (account == null || account.RoleId != 2)
            {
                ModelState.AddModelError("Doctor.AccountId", "Tài khoản không hợp lệ hoặc không phải bác sĩ.");
                return Page();
            }

            var existingDoctor = _doctorService.GetDoctorByAccountId(Doctor.AccountId);
            if (existingDoctor != null)
            {
                ModelState.AddModelError("Doctor.AccountId", "Tài khoản đã được dùng cho bác sĩ khác.");
                return Page();
            }

            string finalImagePath = Doctor.DoctorPath?.Trim();

            if (Doctor.DoctorFile != null && Doctor.DoctorFile.Length > 0)
            {
                var ext = Path.GetExtension(Doctor.DoctorFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Doctor.DoctorFile", "Chỉ cho phép file ảnh định dạng JPG, PNG, GIF.");
                    return Page();
                }

                if (Doctor.DoctorFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Doctor.DoctorFile", "Dung lượng ảnh tối đa là 2MB.");
                    return Page();
                }

                try
                {
                    var wwwRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var folderPath = Path.Combine(wwwRoot, "uploadsDoctor");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var fileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Doctor.DoctorFile.CopyToAsync(stream);
                    }

                    finalImagePath = $"/uploadsDoctor/{fileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Doctor.DoctorFile", "Lỗi khi lưu ảnh: " + ex.Message);
                    return Page();
                }
            }

            var doctorEntity = new User
            {
                FullName = Doctor.FullName,
                Gender = Doctor.Gender,
                DOB = dob,
                Phone = Doctor.Phone,
                Email = Doctor.Email,
                AccountId = Doctor.AccountId,
                DoctorPath = finalImagePath
            };

            _doctorService.CreateDoctor(doctorEntity);
            TempData["SuccessMessage"] = "Tạo bác sĩ thành công.";
            return RedirectToPage("Index");
        }
    }
}
