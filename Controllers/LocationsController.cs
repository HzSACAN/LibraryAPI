using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public LocationsController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Locations
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocation()
        {
          if (_context.Location == null)
          {
              return NotFound();
          }
            return await _context.Location.ToListAsync();
        }

        // GET: api/Locations/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(string id)
        {
          if (_context.Location == null)
          {
              return NotFound();
          }
            var location = await _context.Location.FindAsync(id);

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        // POST: api/Locations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
          if (_context.Location == null)
          {
              return Problem("Entity set 'LibraryAPIContext.Location'  is null.");
          }
            _context.Location.Add(location);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LocationExists(location.Shelf))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLocation", new { id = location.Shelf }, location);
        }

        // DELETE: api/Locations/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("Shelf")]
        public async Task<IActionResult> DeleteLocation(string shelf)
        {
            if (_context.Location == null)
            {
                return NotFound();
            }
            var location = await _context.Location.FindAsync(shelf);
            if (location == null)
            {
                return NotFound();
            }

            location.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(string id)
        {
            return (_context.Location?.Any(e => e.Shelf == id)).GetValueOrDefault();
        }
    }
}
