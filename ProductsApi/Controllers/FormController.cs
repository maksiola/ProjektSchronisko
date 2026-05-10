using Microsoft.AspNetCore.Mvc;
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

        // POST: api/forms
        [HttpPost]
        public async Task<IActionResult> Create(Form form)
        {
            form.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();

            return Ok(form);
        }
    }
}