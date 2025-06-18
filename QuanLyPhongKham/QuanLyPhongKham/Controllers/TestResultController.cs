using BusinessAccessLayer.IService;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace QuanLyPhongKham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        private readonly ITestResultService _testResultService;

        public TestResultController(ITestResultService testResultService)
            => _testResultService = testResultService;

        // OData endpoint - Sửa để include các entity liên quan
        [HttpGet("odata")]
        [EnableQuery]
        public IQueryable<TestResult> GetTestResultsOData()
        {
            try
            {
                // Chuyển đổi List thành IQueryable và để OData tự động load navigation properties
                var testResults = _testResultService.GetAllTestResults().AsQueryable();
                return testResults;
            }
            catch (Exception ex)
            {
                // Log error và return empty queryable
                // Có thể log error tại đây
                return new List<TestResult>().AsQueryable();
            }
        }

        // Main endpoint with filtering support
        [HttpGet]
        public ActionResult<List<TestResultVM>> GetAllTestResults([FromQuery] string? testName = null, [FromQuery] string? userName = null)
        {
            try
            {
                var vm = _testResultService.GetAllTestResultVMs();

                // Filter by test name if provided
                if (!string.IsNullOrEmpty(testName))
                {
                    vm = vm.Where(x => x.TestName != null && x.TestName.Equals(testName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Filter by user name if provided
                if (!string.IsNullOrEmpty(userName))
                {
                    vm = vm.Where(x => x.UserName != null && x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving test results", error = ex.Message });
            }
        }

        // New endpoint to get TestResult entities with ResultId for mapping
        [HttpGet("with-resultid")]
        public ActionResult<List<TestResult>> GetTestResultsWithResultId()
        {
            try
            {
                var testResults = _testResultService.GetAllTestResults();
                return Ok(testResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving test results with ResultId", error = ex.Message });
            }
        }

        // New endpoint to get distinct test names for filter dropdown
        [HttpGet("test-names")]
        public ActionResult<List<string>> GetDistinctTestNames()
        {
            try
            {
                var testResults = _testResultService.GetAllTestResultVMs();
                var distinctTestNames = testResults
                    .Where(x => !string.IsNullOrEmpty(x.TestName))
                    .Select(x => x.TestName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                return Ok(distinctTestNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving test names", error = ex.Message });
            }
        }

        // New endpoint to get distinct user names for filter dropdown
        [HttpGet("user-names")]
        public ActionResult<List<string>> GetDistinctUserNames()
        {
            try
            {
                var testResults = _testResultService.GetAllTestResultVMs();
                var distinctUserNames = testResults
                    .Where(x => !string.IsNullOrEmpty(x.UserName))
                    .Select(x => x.UserName)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                return Ok(distinctUserNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user names", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<TestResult> GetTestResult(int id)
        {
            try
            {
                var tr = _testResultService.GetTestResultById(id);
                if (tr == null) return NotFound(new { message = "Test result not found" });
                return Ok(tr);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving test result", error = ex.Message });
            }
        }

        [HttpGet("record/{recordId}")]
        public ActionResult<List<TestResult>> GetByRecord(int recordId)
        {
            try
            {
                var list = _testResultService.GetTestResultsByRecordId(recordId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving by record", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<TestResult>> GetByUser(int userId)
        {
            try
            {
                var list = _testResultService.GetTestResultsByUserId(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving by user", error = ex.Message });
            }
        }

        [HttpGet("edit/{id}")]
        public ActionResult<TestResultVM> GetForEdit(int id)
        {
            try
            {
                var vm = _testResultService.GetTestResultForEdit(id);
                if (vm == null) return NotFound(new { message = "Not found" });
                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving for edit", error = ex.Message });
            }
        }

        [HttpGet("new")]
        public ActionResult<TestResultVM> GetNew()
        {
            try
            {
                var vm = _testResultService.GetNewTestResultVM();
                return Ok(vm);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating template", error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Create([FromBody] TestResultVM vm)
        {
            try
            {
                if (vm == null) return BadRequest(new { message = "Data required" });

                if (!_testResultService.ValidateTestResult(vm, out var errors))
                    return BadRequest(new { message = "Validation failed", errors });

                if (_testResultService.CreateTestResult(vm))
                    return Ok(new { message = "Created successfully" });

                return StatusCode(500, new { message = "Create failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] TestResultVM vm)
        {
            try
            {
                if (vm == null) return BadRequest(new { message = "Data required" });
                if (!_testResultService.TestResultExists(id))
                    return NotFound(new { message = "Not found" });
                if (!_testResultService.ValidateTestResult(vm, out var errors))
                    return BadRequest(new { message = "Validation failed", errors });

                if (_testResultService.UpdateTestResult(id, vm))
                    return Ok(new { message = "Updated successfully" });

                return StatusCode(500, new { message = "Update failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!_testResultService.TestResultExists(id))
                    return NotFound(new { message = "Not found" });

                if (_testResultService.DeleteTestResult(id))
                    return Ok(new { message = "Deleted successfully" });

                return StatusCode(500, new { message = "Delete failed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting", error = ex.Message });
            }
        }

        [HttpGet("exists/{id}")]
        public ActionResult<bool> Exists(int id)
        {
            try
            {
                var exists = _testResultService.TestResultExists(id);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error checking exists", error = ex.Message });
            }
        }
    }
}