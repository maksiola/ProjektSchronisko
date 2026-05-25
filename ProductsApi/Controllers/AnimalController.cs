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
        public async Task<ActionResult> GetAnimals()
        {
            //ten endpoint musi zwracac anonimowy obiekt, poniewaz byl blad logiczny: potrzebuje zwracac liste zwierzat
            //i wiedziec czy maja carda. z perspektywy frontu nie oplaca sie robic dwoch fetchy, wiec mamy takiego frankensteina
            var animals = await _context.Animals
                 .Include(a => a.Photo)
                .Select(a => new
                {
                    a.AnimalId,
                    a.Species,
                    a.Name,
                    a.Age,
                    a.Sex,
                    a.Description,
                    Photos = a.Photo,
                    Card = _context.Cards 
                        .Where(c => c.AnimalId == a.AnimalId)
                        .Select(c => new { c.Id, c.Status, c.Date })
                        .FirstOrDefault()
                })
                .ToListAsync();
            return Ok(animals);
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