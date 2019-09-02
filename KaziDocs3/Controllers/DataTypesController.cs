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
using Microsoft.AspNetCore.Identity;

namespace KaziDocs3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DataTypesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DataTypesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/DataTypes
        [HttpGet]
        public IEnumerable<DataType> GetDataTypes()
        {

            return _context.DataTypes;
        }

        // GET: api/DataTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataType([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataType = await _context.DataTypes.FindAsync(id);

            if (dataType == null)
            {
                return NotFound();
            }

            return Ok(dataType);
        }

        // PUT: api/DataTypes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataType([FromRoute] string id, [FromBody] DataType dataType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dataType.Id)
            {
                return BadRequest();
            }

            _context.Entry(dataType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataTypeExists(id))
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

        // POST: api/DataTypes
        [HttpPost]
        public async Task<IActionResult> PostDataType([FromBody] DataType dataType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userManager.GetUserId(User);

            dataType.Id = KeyGen.Generate();
            _context.DataTypes.Add(dataType);
            await _context.SaveChangesAsync();

            return StatusCode(200, new 
            {
                Status = 200,
                Message = "Successfully Added",
                Data = dataType
            });
        }

        // DELETE: api/DataTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataType([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dataType = await _context.DataTypes.FindAsync(id);
            if (dataType == null)
            {
                return NotFound();
            }

            _context.DataTypes.Remove(dataType);
            await _context.SaveChangesAsync();

            return Ok(dataType);
        }

        private bool DataTypeExists(string id)
        {
            return _context.DataTypes.Any(e => e.Id == id);
        }
    }
}