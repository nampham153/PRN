using BusinessAccessLayer.IService;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyPhongKham.Pages.Patient
{
    public class EditPatientModel : PageModel
    {
        private readonly IPatientService _patientService;

        public EditPatientModel(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [BindProperty]
        public PatientViewModel PatientViewModel { get; set; }

        public IActionResult OnGet(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }

            PatientViewModel = new PatientViewModel
            {
                PatientId = patient.PatientId,
                FullName = patient.FullName,
                Gender = patient.Gender,
                DOB = (DateTime)patient.DOB,
                Phone = patient.Phone,
                Email = patient.Email,
                Address = patient.Address
            };

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Validation từ ViewModel thất bại
                return Page();
            }

            var patient = _patientService.GetPatientById(PatientViewModel.PatientId);
            if (patient == null)
            {
                return NotFound();
            }

            try
            {
                patient.FullName = PatientViewModel.FullName;
                patient.Gender = PatientViewModel.Gender;
                patient.DOB = PatientViewModel.DOB;
                patient.Phone = PatientViewModel.Phone;
                patient.Email = PatientViewModel.Email;
                patient.Address = PatientViewModel.Address;

                _patientService.UpdatePatient(patient);
                return RedirectToPage("/Patient/PatientList");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); // Hiển thị lỗi từ Service
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật bệnh nhân: " + ex.Message);
                return Page();
            }
        }
    }
}
