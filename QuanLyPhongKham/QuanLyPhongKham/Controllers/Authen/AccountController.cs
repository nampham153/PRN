using BusinessAccessLayer.IService.Authen;
using DataAccessLayer.models;
using DataAccessLayer.ViewModels.Authen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuanLyPhongKham.Controllers.Authen
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IManagerUserService _userService;

        public AccountController(IManagerUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<Role>>> GetRoles()
        {
            var roles = await _userService.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("list")]
        public IActionResult GetAccounts(
            [FromQuery] string search = "",
            [FromQuery] int page = 1,
            [FromQuery] int? roleId = null,
            [FromQuery] bool? status = null)
        {
            var accounts = _userService.GetAccounts(search, page, roleId, status);
            return Ok(accounts);
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountAccounts(
            [FromQuery] string searchKeyword = "",
            [FromQuery] int? roleId = null,
            [FromQuery] bool? status = null)
        {
            var count = await _userService.CountAccountsAsync(searchKeyword, roleId, status);
            return Ok(count);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var acc = _userService.GetAccountById(id);
            return acc != null ? Ok(acc) : NotFound();
        }

        [HttpPost]
        public IActionResult Create([FromBody] UserAccountViewModel account)
        {
            var result = _userService.CreateAccount(account);
            return result ? Ok("Created") : BadRequest("Invalid data or failed to create");
        }

        [HttpPut]
        public IActionResult Update([FromBody] UserAccountViewModel account)
        {
            var result = _userService.UpdateAccount(account);
            return result ? Ok("Updated") : NotFound("Account not found");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _userService.DeleteAccount(id);
            return result ? Ok("Deleted") : BadRequest("Account is active or not found");
        }
    }


}
