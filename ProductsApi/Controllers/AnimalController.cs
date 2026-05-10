using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnimalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {
            return await _context.Animals
                .Include(a => a.Photo) // Dołączamy listę zdjęć do każdego zwierzaka
                .ToListAsync();
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            // Używamy FirstOrDefaultAsync zamiast FindAsync, bo Include nie działa z FindAsync
            var animal = await _context.Animals
                .Include(a => a.Photo)
                .FirstOrDefaultAsync(a => a.AnimalId == id);

            if (animal == null) return NotFound();

            return animal;
        }
    }
}