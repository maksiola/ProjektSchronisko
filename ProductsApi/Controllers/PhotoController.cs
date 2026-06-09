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
        public PhotosController(AppDbContext context) => _context = context;

        [HttpGet("main")]
        public async Task<IActionResult> GetOnePhotoForAnimal()
        {
            try
            {
                var photos = await _context.Photos
                    .GroupBy(p => p.AnimalId)
                    .Select(g => g.OrderByDescending(p => p.Main).ThenBy(p => p.Id).First())
                    .ToListAsync();
                return Ok(photos);
            }
            catch (Exception ex) { return StatusCode(500, $"Błąd pobierania: {ex.Message}"); }
        }

        [HttpGet("animal/{animalId}")]
        public async Task<IActionResult> GetPhotosByAnimal(int animalId)
        {
            try
            {
                var photos = await _context.Photos.Where(p => p.AnimalId == animalId).ToListAsync();
                return photos.Count > 0 ? Ok(photos) : NotFound($"Brak zdjęć dla zwierzaka {animalId}");
            }
            catch (Exception ex) { return StatusCode(500, $"Błąd serwera: {ex.Message}"); }
        }

        [HttpPost("animal/{animalId}")]
        public IActionResult UploadPhotosForAnimal(int animalId, [FromForm] List<IFormFile> photos)
        {
            try
            {
                if (photos == null || photos.Count == 0) return BadRequest("Brak dodanych zdjęć.");

                foreach (var photo in photos)
                {
                    if (!photo.ContentType.StartsWith("image/")) return BadRequest($"Plik {photo.FileName} to nie zdjęcie.");
                    Console.WriteLine($"{photo.FileName} | {photo.Length} bytes");
                }

                return Ok(new { animalId, photosCount = photos.Count });
            }
            catch (Exception ex) { return StatusCode(500, $"Błąd zapisu: {ex.Message}"); }
        }
        [HttpDelete("animal/{animalId}")]
        public async Task<IActionResult> DeletePhotosByAnimal(int animalId)
        {
            try
            {
                var photos = await _context.Photos.Where(p => p.AnimalId == animalId).ToListAsync();

                if (photos.Count > 0)
                {
                    _context.Photos.RemoveRange(photos);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Zdjęcia zostały usunięte" });
            }
            catch (Exception ex) { return StatusCode(500, $"Błąd usuwania: {ex.Message}"); }
        }
    }
}