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
    public class FormNamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormNamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FormNames
        [HttpGet]
        public IEnumerable<FormName> GetFormNames()
        {
            return _context.FormNames;
        }

        // GET: api/FormNames/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormName([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formName = await _context.FormNames.FindAsync(id);

            if (formName == null)
            {
                return NotFound();
            }

            return Ok(formName);
        }

        // PUT: api/FormNames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormName([FromRoute] string id, [FromBody] FormName formName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != formName.Id)
            {
                return BadRequest();
            }

            _context.Entry(formName).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormNameExists(id))
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

        // POST: api/FormNames
        [HttpPost]
        public async Task<IActionResult> PostFormName([FromBody] FormName formName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FormNames.Add(formName);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormName", new { id = formName.Id }, formName);
        }

        // DELETE: api/FormNames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormName([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formName = await _context.FormNames.FindAsync(id);
            if (formName == null)
            {
                return NotFound();
            }

            _context.FormNames.Remove(formName);
            await _context.SaveChangesAsync();

            return Ok(formName);
        }

        private bool FormNameExists(string id)
        {
            return _context.FormNames.Any(e => e.Id == id);
        }
    }
}