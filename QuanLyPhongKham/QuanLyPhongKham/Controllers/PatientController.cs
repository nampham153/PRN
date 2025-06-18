using BusinessAccessLayer.IService;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PatientViewModel>> GetAllPatients()
        {
            var patients = _patientService.GetAllPatients();
            var patientViewModels = patients.Select(p => new PatientViewModel
            {
                PatientId = p.PatientId,
                FullName = p.FullName,
                Gender = p.Gender,
                DOB = (DateTime)p.DOB,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address,
                MedicalRecordCount = p.MedicalRecords?.Count ?? 0
            }).ToList();
            return Ok(patientViewModels);
        }

        [HttpGet("{id}")]
        public ActionResult<PatientViewModel> GetPatientById(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(new PatientViewModel
            {
                PatientId = patient.PatientId,
                FullName = patient.FullName,
                Gender = patient.Gender,
                DOB = (DateTime)patient.DOB,
                Phone = patient.Phone,
                Email = patient.Email,
                Address = patient.Address,
                MedicalRecordCount = patient.MedicalRecords?.Count ?? 0
            });
        }

        [HttpPost]
        public ActionResult AddPatient([FromBody] PatientViewModel patientViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = new DataAccessLayer.models.Patient
            {
                FullName = patientViewModel.FullName,
                Gender = patientViewModel.Gender,
                DOB = patientViewModel.DOB,
                Phone = patientViewModel.Phone,
                Email = patientViewModel.Email,
                Address = patientViewModel.Address
            };

            _patientService.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.PatientId }, patientViewModel);
        }

        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] PatientViewModel patientViewModel)
        {
            if (id != patientViewModel.PatientId || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.FullName = patientViewModel.FullName;
            patient.Gender = patientViewModel.Gender;
            patient.DOB = patientViewModel.DOB;
            patient.Phone = patientViewModel.Phone;
            patient.Email = patientViewModel.Email;
            patient.Address = patientViewModel.Address;

            _patientService.UpdatePatient(patient);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }
            _patientService.DeletePatient(id);
            return NoContent();
        }
    }
}
