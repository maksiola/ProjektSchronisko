using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CardsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            //try catch
            try
            {
                var cards = await _context.Cards
                    .Include(c => c.Animal)
                        .ThenInclude(a => a.Photo.Where(p => p.Main == true))
                    .ToListAsync();

                return Ok(new
                {
                    code = 200,
                    message = "Poprawnie załadowano karty",
                    data = cards
                });
            }
            catch (Exception ex)
            {
                //blad serw
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Błąd w trakcie ładowania kart",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCard(int id)
        {
            //try catch
            try
            {
                var card = await _context.Cards
                    .Include(c => c.Animal)
                        .ThenInclude(a => a.Photo.Where(p => p.Main == true))
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (card == null)
                {
                    return NotFound(new
                    {
                        code = 404,
                        message = $"Karta z {id} nie została znaleziona"
                    });
                }

                return Ok(new
                {
                    code = 200,
                    message = "Poprawnie załadowano kartę",
                    data = card
                });
            }
            catch (Exception ex)
            {
                //blad serw
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Błąd w trakcie ładowania kart",
                    error = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostCard(Card card)
        {
            try
            {
                // Czy body jest puste
                if (card == null)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        message = "Dane karty są niezbędne"
                    });
                }

                // --- NOWA LOGIKA PRZETWARZANIA BASE64 DLA ZDJĘĆ ZWIERZAKA W KARCIE ---
                if (card.Animal != null && card.Animal.Photo != null)
                {
                    foreach (var photo in card.Animal.Photo)
                    {
                        // Resetujemy ID, aby baza danych przypisała je automatycznie (Identity)
                        photo.Id = 0;

                        if (!string.IsNullOrEmpty(photo.Base64Data))
                        {
                            string base64String = photo.Base64Data;

                            // Pozbywamy się nagłówka np. "data:image/png;base64,"
                            if (base64String.Contains(","))
                            {
                                base64String = base64String.Split(',')[1];
                            }

                            try
                            {
                                // Konwersja ciągu tekstowego na surowe bajty do bazy SQL
                                photo.ImageData = Convert.FromBase64String(base64String);
                            }
                            catch (FormatException)
                            {
                                return BadRequest(new
                                {
                                    code = 400,
                                    message = $"Niepoprawny format ciągu Base64 w zdjęciu dla zwierzęcia {card.Animal.Name}."
                                });
                            }
                        }
                    }
                }
                // --------------------------------------------------------------------

                _context.Cards.Add(card);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetCard),
                    new { id = card.Id },
                    new
                    {
                        code = 201,
                        message = "Pomyślnie utworzono kartę wraz z danymi zwierzęcia",
                        data = card
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Błąd w trakcie zapisywania karty",
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            try
            {
                var card = await _context.Cards
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (card == null)
                {
                    return NotFound(new { code = 404, message = $"Karta z id {id} nie została znaleziona" });
                }

                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();

                return Ok(new { code = 200, message = $"Pomyślnie usunięto kartę z id {id}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = "Błąd w trakcie usuwania karty", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, Card card)
        {
            if (id != card.Id)
                return BadRequest();

            card.Animal = null;
            _context.Entry(card).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cards.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("animal/{animalId}")]
        public async Task<IActionResult> DeleteCardByAnimalId(int animalId)
        {
            try
            {
                var card = await _context.Cards.FirstOrDefaultAsync(c => c.AnimalId == animalId);

                if (card == null)
                    return Ok(new { code = 200, message = "Brak karty dla tego zwierzęcia" });

                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();

                return Ok(new { code = 200, message = $"Pomyślnie usunięto kartę dla zwierzęcia {animalId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = "Błąd usuwania karty", error = ex.Message });
            }
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }
    }
}