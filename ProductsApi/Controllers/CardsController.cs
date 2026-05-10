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

        // GET: api/Cards
        // Pobiera wszystkie karty na kafelki - tylko ze zdjęciem głównym
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Card>>> GetAllCards()
        {
            return await _context.Cards
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Photo.Where(p => p.Main == true))
                .ToListAsync();
        }

        // GET: api/Cards/5
        // Pobiera szczegóły jednej karty - tutaj też tylko zdjęcie główne
        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            var card = await _context.Cards
                .Include(c => c.Animal)
                    .ThenInclude(a => a.Photo.Where(p => p.Main == true))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null)
            {
                return NotFound();
            }

            return card;
        }
    }
}