using BusinessAccessLayer.IService;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyPhongKham.Pages.Patient
{
    public class DeletePatientModel : PageModel
    {
        private readonly IPatientService _patientService;

        public DeletePatientModel(IPatientService patientService)
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

        public IActionResult OnPost(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient != null)
            {
                _patientService.DeletePatient(id);
            }

            return RedirectToPage("/Patient/PatientList");
        }
    }
}
