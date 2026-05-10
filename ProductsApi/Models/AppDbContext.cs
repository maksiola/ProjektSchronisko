using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<Photo> Photos { get; set; }
}