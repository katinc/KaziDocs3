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
    public class FormTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FormTypes
        [HttpGet]
        public IEnumerable<FormType> GetFormTypes()
        {
            return _context.FormTypes;
        }

        // GET: api/FormTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFormType([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formType = await _context.FormTypes.FindAsync(id);

            if (formType == null)
            {
                return NotFound();
            }

            return Ok(formType);
        }

        // PUT: api/FormTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFormType([FromRoute] string id, [FromBody] FormType formType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != formType.Id)
            {
                return BadRequest();
            }

            _context.Entry(formType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FormTypeExists(id))
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

        // POST: api/FormTypes
        [HttpPost]
        public async Task<IActionResult> PostFormType([FromBody] FormType formType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.FormTypes.Add(formType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFormType", new { id = formType.Id }, formType);
        }

        // DELETE: api/FormTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFormType([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var formType = await _context.FormTypes.FindAsync(id);
            if (formType == null)
            {
                return NotFound();
            }

            _context.FormTypes.Remove(formType);
            await _context.SaveChangesAsync();

            return Ok(formType);
        }

        private bool FormTypeExists(string id)
        {
            return _context.FormTypes.Any(e => e.Id == id);
        }
    }
}