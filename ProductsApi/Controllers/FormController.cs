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

        // GET: api/Forms
        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            try
            {
                var forms = await _context.Forms
                    .OrderByDescending(f => f.Id)
                    .ToListAsync();

                return Ok(new
                {
                    code = 200,
                    message = "Forms loaded successfully.",
                    data = forms
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Error while loading forms.",
                    error = ex.Message
                });
            }
        }

        // POST: api/Forms
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
                        message = "Form data is required."
                    });
                }

                form.Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                _context.Forms.Add(form);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    code = 200,
                    message = "Form submitted successfully.",
                    data = form
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    message = "Error while submitting form.",
                    error = ex.Message
                });
            }
        }
    }
}