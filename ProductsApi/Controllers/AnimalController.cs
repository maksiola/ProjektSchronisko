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

        private async Task<int> GetNextFreeAnimalId()
        {
            const int startId = 1;

            var usedIds = await _context.Animals
                .Select(a => a.AnimalId)
                .ToListAsync();

            var newId = startId;

            while (usedIds.Contains(newId))
            {
                newId++;
            }

            return newId;
        }

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

        //search?query=xxx
        [HttpGet("search")]
        public async Task<ActionResult> SearchAnimals(
         [FromQuery] string? query,
         [FromQuery] string? species,
         [FromQuery] char? sex)
        {
            var animalsQuery = _context.Animals
                .Include(a => a.Photo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
                animalsQuery = animalsQuery.Where(a => a.Name.StartsWith(query));

            if (!string.IsNullOrWhiteSpace(species))
                animalsQuery = animalsQuery.Where(a => a.Species == species);

            if (sex.HasValue)
                animalsQuery = animalsQuery.Where(a => a.Sex == sex.Value);

            var animals = await animalsQuery
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, [FromBody] Animal animal)
        {
            if (id != animal.AnimalId) return BadRequest();

            var allowedSpecies = new[] { "Kot", "Pies" };
            if (!allowedSpecies.Contains(animal.Species))
                return BadRequest("'Kot' or 'Pies'");

            if (animal.Sex != 'M' && animal.Sex != 'F')
                return BadRequest("'M' or 'F'");

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Animals.Any(a => a.AnimalId == id)) return NotFound();
                throw;
            }

            return Ok(animal);
        }

        [HttpPost]
        public async Task<ActionResult<Animal>> CreateAnimal(Animal animal)
        {
            var allowedSpecies = new[] { "Kot", "Pies" };
            if (!allowedSpecies.Contains(animal.Species))
                return BadRequest("'Kot' or 'Pies'");

            if (animal.Sex != 'M' && animal.Sex != 'F')
                return BadRequest("'M' or 'F'");

            if (animal.AnimalId == 0)
            {
                animal.AnimalId = await GetNextFreeAnimalId();
            }

            var animalExists = await _context.Animals
                .AnyAsync(a => a.AnimalId == animal.AnimalId);

            if (animalExists)
            {
                return BadRequest($"Animal with ID {animal.AnimalId} already exists.");
            }

            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAnimal),
                new { id = animal.AnimalId },
                animal
            );
        }

        // DELETE: api/Animals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            var animal = await _context.Animals.FindAsync(id);

            if (animal == null)
            {
                return NotFound();
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}