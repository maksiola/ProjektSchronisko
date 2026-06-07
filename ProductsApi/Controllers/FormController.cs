using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FormsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/forms
        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var forms = await _context.Forms.ToListAsync();

                // sprawdza czy w bazie są jakieś formularze
                if (forms == null || !forms.Any())
                {
                    return NotFound(new { code = 404, message = "Brak formularzy." });
                }

                return Ok(forms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = $"Błąd podczas pobierania formularzy: {ex.Message}" });
            }
        }

        // POST: api/forms
        [HttpPost]
        public async Task<IActionResult> Create(Form form)
        {
            try
            {
                if (form == null)
                {
                    return BadRequest(new
                    {
                        code = 400,
                        message = "Dane formularza są obowiązkowe"
                    });
                }

                form.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                _context.Forms.Add(form);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    code = 200,
                    message = "Poparwnie dodano formularz",
                    data = form
                });
            }
            catch (Exception ex)
            {
                //blad serw
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Błąd w trakcie dodawania formularza",
                    error = ex.Message
                });
            }
        }
    }
}
