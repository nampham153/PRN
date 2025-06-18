using DataAccessLayer.models;
using Microsoft.AspNetCore.Http; // Để dùng IFormFile
using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.ViewModels
{
    public class DoctorVM
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ và tên phải từ 3 đến 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [RegularExpression("Male|Female|Other", ErrorMessage = "Giới tính phải là Male, Female hoặc Other")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email chưa đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "AccountId là bắt buộc")]
        public int AccountId { get; set; }
        public IFormFile? DoctorFile { get; set; }
        public string? DoctorPath { get; set; } 

        public static User ToEntity(DoctorVM vm)
        {
            return new User
            {
                UserId = vm.UserId,
                FullName = vm.FullName,
                Gender = vm.Gender,
                DOB = vm.DOB,
                Phone = vm.Phone,
                Email = vm.Email,
                AccountId = vm.AccountId,
                DoctorPath = vm.DoctorPath
            };
        }
    }
}
