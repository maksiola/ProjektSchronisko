using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.Services;

namespace ProductsApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<Employee> Authenticate([FromBody] AuthenticateRequest model)
        {
            var employee = _userService.Authenticate(model.Username, model.Password);

            if (employee == null)
                return BadRequest(new { message = "Nazwa użytkownika lub hasło są niepoprawne" });

            return Ok(employee);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<Employee> Register([FromBody] Employee employeeParam)
        {
            try
            {
                var newEmployee = _userService.Register(employeeParam);
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            var employees = _userService.GetAll();
            return Ok(employees);
        }
    }

    public class AuthenticateRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}