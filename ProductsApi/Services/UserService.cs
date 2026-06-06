using Microsoft.Extensions.Options;
using ProductsApi.Models;
using ProductsApi.Utilities;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace ProductsApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly AppSettings _appSettings;

        public UserService(AppDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public Employee? Authenticate(string username, string password)
        {
            var employee = _context.Employees.SingleOrDefault(x => x.Mail == username && x.Password == password);

            if (employee == null)
                return null;

            
            employee.Token = GenerateToken(employee.Id.ToString());

            
            employee.Password = string.Empty;

            return employee;
        }

        public IEnumerable<Employee> GetAll()
        {
            
            return _context.Employees.Select(x => new Employee
            {
                Id = x.Id,
                Name = x.Name,
                LastName = x.LastName,
                Mail = x.Mail,
                Role = x.Role,
                Token = x.Token
            }).ToList();
        }

        private string GenerateToken(string employeeId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, employeeId)
                }),
                Expires = DateTime.UtcNow.AddDays(7), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public Employee Register(Employee employee)
        {
           
            var alreadyExists = _context.Employees.Any(x => x.Mail == employee.Mail);
            if (alreadyExists)
            {
                throw new Exception("Pracownik o podanym adresie E-mail już istnieje w systemie.");
            }

            
            _context.Employees.Add(employee);
           
            _context.SaveChanges();

            employee.Password = string.Empty;

            return employee;
        }
    }
}