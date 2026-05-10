using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    [ApiController]
    [Route("adminpanel/[controller]")]

    public class PhotosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PhotosController(AppDbContext context)
        {
            _context = context;
        }

        //GET - jedno zdjecie
        [HttpGet("main")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetOnePhotoForAnimal()
        {
            var photos = await _context.Photos
                .GroupBy(p => p.AnimalId)
                .Select(g => g
                    .OrderByDescending(p => p.Main)
                    .ThenBy(p => p.Id)
                    .First())
                .ToListAsync();
            return Ok(photos);
        }
        //GET - wszystkie zdjecia
        [HttpGet("animal/{animalId}")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosByAnimal(int animalId)
        {
            var photos = await _context.Photos
                .Where(p => p.AnimalId == animalId)
                .ToListAsync();
            if (photos.Count <= 0)
            {
                return NotFound();
            }
            return Ok(photos);
        }
    }
}