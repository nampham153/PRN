using BusinessAccessLayer.IService;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyPhongKham.Pages.Patient
{
    public class CreatePatientModel : PageModel
    {
        private readonly IPatientService _patientService;

        public CreatePatientModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public PatientViewModel PatientViewModel { get; set; }

        public IActionResult OnGet()
        {
            PatientViewModel = new PatientViewModel();
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var patient = new DataAccessLayer.models.Patient
                {
                    FullName = PatientViewModel.FullName,
                    Gender = PatientViewModel.Gender,
                    DOB = PatientViewModel.DOB,
                    Phone = PatientViewModel.Phone,
                    Email = PatientViewModel.Email,
                    Address = PatientViewModel.Address
                };

                _patientService.AddPatient(patient);
                return RedirectToPage("/Patient/PatientList");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); // Hiển thị lỗi từ Service
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi thêm bệnh nhân: " + ex.Message);
                return Page();
            }
        }
    }
}
