using Microsoft.AspNetCore.Authorization;
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
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Błąd serwera, nie pobrało zwierząt",
                    error = ex.Message
                });
            }
        }

        // GET: api/Animals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Animal>> GetAnimal(int id)
        {
            try
            {
                var animal = await _context.Animals
                    .Include(a => a.Photo)
                    .FirstOrDefaultAsync(a => a.AnimalId == id);

                if (animal == null)
                {
                    return NotFound(new
                    {
                        message = $"Nie znaleziono zwierzęcia o ID {id}"
                    });
                }

                return Ok(animal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Błąd podczas pobierania",
                    error = ex.Message
                });
            }
        }



        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, [FromBody] Animal animal)
        {
            if (id != animal.AnimalId) return BadRequest();

            var allowedSpecies = new[] { "Kot", "Pies" };
            if (!allowedSpecies.Contains(animal.Species))
                return BadRequest("'Kot' or 'Pies'");

            if (animal.Sex != 'M' && animal.Sex != 'F')
                return BadRequest("'M' or 'F'");

            if (animal.Photo != null)
            {
                foreach (var photo in animal.Photo)
                {
                    photo.AnimalId = id;

                    if (!string.IsNullOrEmpty(photo.Base64Data))
                    {
                        string base64String = photo.Base64Data;
                        if (base64String.Contains(","))
                        {
                            base64String = base64String.Split(',')[1];
                        }

                        try
                        {

                            photo.ImageData = Convert.FromBase64String(base64String);
                        }
                        catch (FormatException)
                        {
                            return BadRequest("Niepoprawny format Base64");
                        }
                    }
                }
            }


            _context.Update(animal);

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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Animal>> CreateAnimal(Animal animal)
        {
            if (animal == null) return BadRequest("Brak danych zwierzaka.");

            var allowedSpecies = new[] { "Kot", "Pies" };
            if (!allowedSpecies.Contains(animal.Species))
                return BadRequest("'Kot' or 'Pies'");

            if (animal.Sex != 'M' && animal.Sex != 'F')
                return BadRequest("'M' or 'F'");


            animal.AnimalId = 0;

            if (animal.Photo != null)
            {
                foreach (var photo in animal.Photo)
                {
                    Console.WriteLine(photo);

                    photo.Id = 0;
                    photo.AnimalId = 0;

                    if (!string.IsNullOrEmpty(photo.Base64Data))
                    {
                        string base64String = photo.Base64Data;
                        if (base64String.Contains(","))
                        {
                            base64String = base64String.Split(',')[1];
                        }

                        try
                        {
                            photo.ImageData = Convert.FromBase64String(base64String);
                        }
                        catch (FormatException)
                        {
                            return BadRequest("Niepoprawny format ciągu Base64 w dodawanym zdjęciu.");
                        }
                    }
                }
            }

            try
            {
                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetAnimal),
                    new { id = animal.AnimalId },
                    new
                    {
                        message = "utworzono zwierze",
                        animal
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Błąd podczas tworzenia zwierzęcia",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            try
            {
                var animal = await _context.Animals.FindAsync(id);

                if (animal == null)
                {
                    return NotFound(new
                    {
                        message = $"Nie znaleziono zwierzęcia z ID {id}"
                    });
                }

                _context.Animals.Remove(animal);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Zwierzę zostało usunięte"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Błąd podczas usuwania",
                    error = ex.Message,
                     inner = ex.InnerException?.Message
                });
            }
        }
    }
    }