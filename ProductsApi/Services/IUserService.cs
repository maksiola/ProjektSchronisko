using ProductsApi.Models;

public interface IUserService
{
    Employee? Authenticate(string username, string password);
    IEnumerable<Employee> GetAll();
    Employee Register(Employee employee);
}