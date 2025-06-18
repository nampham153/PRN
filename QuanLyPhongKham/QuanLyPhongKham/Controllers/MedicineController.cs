using BusinessAccessLayer.IService;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        // GET: api/Medicine
        [HttpGet]
        public IActionResult GetAllMedicines()
        {
            try
            {
                var medicines = _medicineService.GetAllMedicines();
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Medicine/5
        [HttpGet("{id}")]
        public IActionResult GetMedicine(int id)
        {
            try
            {
                var medicine = _medicineService.GetMedicineById(id);
                if (medicine == null)
                    return NotFound($"Medicine with ID {id} not found");

                return Ok(medicine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Medicine
        [HttpPost]
        public IActionResult CreateMedicine([FromBody] MedicineVM medicineVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var medicine = _medicineService.CreateMedicine(medicineVM);
                return CreatedAtAction(nameof(GetMedicine), new { id = medicine.MedicineId }, medicine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Medicine/5
        [HttpPut("{id}")]
        public IActionResult UpdateMedicine(int id, [FromBody] MedicineVM medicineVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var medicine = _medicineService.UpdateMedicine(id, medicineVM);
                if (medicine == null)
                    return NotFound($"Medicine with ID {id} not found");

                return Ok(medicine);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Medicine/5
        [HttpDelete("{id}")]
        public IActionResult DeleteMedicine(int id)
        {
            try
            {
                var result = _medicineService.DeleteMedicine(id);
                if (!result)
                    return NotFound($"Medicine with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Medicine/search?term=aspirin
        [HttpGet("search")]
        public IActionResult SearchMedicines([FromQuery] string term)
        {
            try
            {
                var medicines = _medicineService.SearchMedicines(term);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Medicine/by-name?name=aspirin
        [HttpGet("by-name")]
        public IActionResult GetMedicinesByName([FromQuery] string name)
        {
            try
            {
                var medicines = _medicineService.GetMedicinesByName(name);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Medicine/by-unit?unit=tablet
        [HttpGet("by-unit")]
        public IActionResult GetMedicinesByUnit([FromQuery] string unit)
        {
            try
            {
                var medicines = _medicineService.GetMedicinesByUnit(unit);
                return Ok(medicines);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Medicine/count
        [HttpGet("count")]
        public IActionResult GetTotalCount()
        {
            try
            {
                var count = _medicineService.GetTotalMedicinesCount();
                return Ok(new { TotalCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}