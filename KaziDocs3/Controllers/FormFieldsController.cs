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
    public class FormFieldsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormFieldsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FormFields
        [HttpGet]
        public IEnumerable<FormField> GetFormFields()
        {
            return _context.FormFields;
        }

        // GET: api/FormFields/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormField([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formField = await _context.FormFields.FindAsync(id);

            if (formField == null)
            {
                return NotFound();
            }

            return Ok(formField);
        }

        // PUT: api/FormFields/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormField([FromRoute] string id, [FromBody] FormField formField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != formField.Id)
            {
                return BadRequest();
            }

            _context.Entry(formField).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormFieldExists(id))
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

        // POST: api/FormFields
        [HttpPost]
        public async Task<IActionResult> PostFormField([FromBody] FormField formField)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FormFields.Add(formField);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormField", new { id = formField.Id }, formField);
        }

        // DELETE: api/FormFields/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormField([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formField = await _context.FormFields.FindAsync(id);
            if (formField == null)
            {
                return NotFound();
            }

            _context.FormFields.Remove(formField);
            await _context.SaveChangesAsync();

            return Ok(formField);
        }

        private bool FormFieldExists(string id)
        {
            return _context.FormFields.Any(e => e.Id == id);
        }
    }
}