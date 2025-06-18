using BusinessAccessLayer.IService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly IDoctorService _doctorService;

        public IndexModel(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        public List<DoctorVM> Doctors { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public void OnGet()
        {
            var doctors = _doctorService.GetAllDoctors();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                doctors = doctors
                    .Where(d => d.FullName != null && d.FullName.Contains(SearchTerm, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Doctors = doctors.Select(d => new DoctorVM
            {
                UserId = d.UserId,
                FullName = d.FullName,
                Gender = d.Gender,
                DOB = d.DOB,
                Phone = d.Phone,
                Email = d.Email,
                AccountId = d.AccountId
            }).ToList();
        }
    }
}
