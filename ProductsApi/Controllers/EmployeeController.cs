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
            // sprawdza czy jest pusto
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { code = 400, message = "Nazwa i hasło muszą być wpisane." });
            }

            try
            {
                var employee = _userService.Authenticate(model.Username, model.Password);

                if (employee == null)
                {
                    return BadRequest(new { code = 400, message = "Nazwa lub hasło są niepoprawne." });
                }

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = $"Wystąpił błąd podczas autentykacji: {ex.Message}" });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<Employee> Register([FromBody] Employee employeeParam)
        {
          
            if (employeeParam == null)
            {
                return BadRequest(new { code = 400, message = "Dane do rejestracji muszą być uzupełnione." });
            }

            try
            {
                var newEmployee = _userService.Register(employeeParam);
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            try
            {
                var employees = _userService.GetAll();

                // jeśli lista jest pusta zwraca kod 404 zamiast pustej listy
                if (employees == null || !employees.Any())
                {
                    return NotFound(new { code = 404, message = "Nie znaleziono użytkowników." });
                }

                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = $"Nie udało się pobrać użytkowników: {ex.Message}" });
            }
        }
    }

    public class AuthenticateRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
