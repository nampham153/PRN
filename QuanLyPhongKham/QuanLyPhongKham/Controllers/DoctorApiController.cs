using BusinessAccessLayer.IService;
using DataAccessLayer.ViewModels;
using DataAccessLayer.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.IRepository;

namespace QuanLyPhongKham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorApiController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IAccountRepository _accountRepository;

        public DoctorApiController(IDoctorService doctorService, IAccountRepository accountRepository)
        {
            _doctorService = doctorService;
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public IActionResult GetAllDoctors()
        {
            var doctors = _doctorService.GetAllDoctors();
            return Ok(doctors);
        }

        [HttpGet("{accountId}")]
        public IActionResult GetDoctorByAccountId(int accountId)
        {
            var doctor = _doctorService.GetDoctorByAccountId(accountId);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromForm] DoctorVM doctorVM)
        {
            var existingDoctor = _doctorService.GetDoctorByAccountId(doctorVM.AccountId);
            if (existingDoctor != null)
            {
                return BadRequest("Tài khoản này đã được đăng ký.");
            }

            var account = _accountRepository.GetAccountById(doctorVM.AccountId);
            if (account == null || account.RoleId != 2)
            {
                return BadRequest("Vui lòng nhập lại AccountId.");
            }

            // Xử lý upload ảnh
            if (doctorVM.DoctorFile != null && doctorVM.DoctorFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(doctorVM.DoctorFile.FileName);
                var filePath = Path.Combine("wwwroot", "uploadsDoctor", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await doctorVM.DoctorFile.CopyToAsync(stream);
                }

                doctorVM.DoctorPath = $"/uploadsDoctor/{fileName}";
            }

            var doctorEntity = new User
            {
                FullName = doctorVM.FullName,
                Gender = doctorVM.Gender,
                DOB = doctorVM.DOB,
                Phone = doctorVM.Phone,
                Email = doctorVM.Email,
                AccountId = doctorVM.AccountId,
                DoctorPath = doctorVM.DoctorPath
            };

            _doctorService.CreateDoctor(doctorEntity);
            return Ok("Tạo bác sĩ thành công.");
        }


        [HttpPut("{accountId}")]
        public IActionResult UpdateDoctor(int accountId, [FromBody] DoctorVM doctorVM)
        {
            var doctorEntity = new User
            {
                UserId = doctorVM.UserId,
                FullName = doctorVM.FullName,
                Gender = doctorVM.Gender,
                DOB = doctorVM.DOB,
                Phone = doctorVM.Phone,
                Email = doctorVM.Email,
                AccountId = doctorVM.AccountId
            };

            _doctorService.UpdateDoctor(accountId, doctorEntity);
            return Ok();
        }

        [HttpDelete("{accountId}")]
        public IActionResult DeleteDoctor(int accountId)
        {
            _doctorService.DeleteDoctor(accountId);
            return Ok();
        }
    }
}
