using BusinessAccessLayer.IService;
using DataAccessLayer.ViewModels;
using DataAccessLayer.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        // GET: api/Test
        [HttpGet]
        public IActionResult GetAllTests()
        {
            try
            {
                var tests = _testService.GetAllTests();
                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/Test/search?searchTerm=keyword
        [HttpGet("search")]
        public IActionResult SearchTests([FromQuery] string searchTerm)
        {
            try
            {
                var tests = _testService.SearchTests(searchTerm);
                return Ok(new
                {
                    searchTerm = searchTerm,
                    count = tests.Count,
                    data = tests
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/Test/filter?searchTerm=keyword&sortBy=TestName&sortDescending=false&pageNumber=1&pageSize=10
        [HttpGet("filter")]
        public IActionResult GetTestsWithFilter([FromQuery] SearchFilterVM filter)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = _testService.GetTestsWithFilter(filter ?? new SearchFilterVM());
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/Test/advanced-search - cho complex search
        [HttpPost("advanced-search")]
        public IActionResult AdvancedSearch([FromBody] SearchFilterVM searchFilter)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = _testService.GetTestsWithFilter(searchFilter);
                return Ok(result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/Test/5
        [HttpGet("{id}")]
        public IActionResult GetTest(int id)
        {
            try
            {
                var test = _testService.GetTestById(id);
                return Ok(test);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/Test
        [HttpPost]
        public IActionResult CreateTest([FromBody] TestVM testVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _testService.CreateTest(testVM);
                return Ok(new { message = "Test created successfully" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // PUT: api/Test/5
        [HttpPut("{id}")]
        public IActionResult UpdateTest(int id, [FromBody] TestVM testVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _testService.UpdateTest(id, testVM);
                return Ok(new { message = "Test updated successfully" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        // DELETE: api/Test/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTest(int id)
        {
            try
            {
                _testService.DeleteTest(id);
                return Ok(new { message = "Test deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}