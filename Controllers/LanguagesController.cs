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
    public class LanguagesController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public LanguagesController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Languages
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguage()
        {
          if (_context.Language == null)
          {
              return NotFound();
          }
            return await _context.Language.ToListAsync();
        }

        // GET: api/Languages/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(string id)
        {
          if (_context.Language == null)
          {
              return NotFound();
          }
            var language = await _context.Language.FindAsync(id);

            if (language == null)
            {
                return NotFound();
            }

            return language;
        }

        // POST: api/Languages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Employee")]
        [HttpPost]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
          if (_context.Language == null)
          {
              return Problem("Entity set 'LibraryAPIContext.Language'  is null.");
          }
            _context.Language.Add(language);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LanguageExists(language.Code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLanguage", new { id = language.Code }, language);
        }

        // DELETE: api/Languages/5
        [Authorize(Roles ="Employee")]
        [HttpDelete("Code")]
        public async Task<IActionResult> DeleteLanguage(string code)
        {
            if (_context.Language == null)
            {
                return NotFound();
            }
            var language = await _context.Language.FindAsync(code);
            if (language == null)
            {
                return NotFound();
            }

            language.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguageExists(string id)
        {
            return (_context.Language?.Any(e => e.Code == id)).GetValueOrDefault();
        }
    }
}
