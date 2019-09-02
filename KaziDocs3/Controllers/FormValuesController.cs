using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KaziDocs3.Data;
using KaziDocs3.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace KaziDocs3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FormValuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormValuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FormValues
        [HttpGet]
        public IEnumerable<FormValue> GetFormValues()
        {
            return _context.FormValues;
        }

        // GET: api/FormValues/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormValue([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formValue = await _context.FormValues.FindAsync(id);

            if (formValue == null)
            {
                return NotFound();
            }

            return Ok(formValue);
        }

        // PUT: api/FormValues/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormValue([FromRoute] string id, [FromBody] FormValue formValue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != formValue.Id)
            {
                return BadRequest();
            }

            _context.Entry(formValue).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormValueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FormValues
        [HttpPost]
        public async Task<IActionResult> PostFormValue([FromBody] FormValue formValue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FormValues.Add(formValue);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormValue", new { id = formValue.Id }, formValue);
        }

        // DELETE: api/FormValues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormValue([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formValue = await _context.FormValues.FindAsync(id);
            if (formValue == null)
            {
                return NotFound();
            }

            _context.FormValues.Remove(formValue);
            await _context.SaveChangesAsync();

            return Ok(formValue);
        }

        private bool FormValueExists(string id)
        {
            return _context.FormValues.Any(e => e.Id == id);
        }
    }
}