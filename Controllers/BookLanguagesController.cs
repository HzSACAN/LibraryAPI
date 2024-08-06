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
    public class BookLanguagesController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public BookLanguagesController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/BookLanguages
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLanguage>>> GetBookLanguage()
        {
          if (_context.BookLanguage == null)
          {
              return NotFound();
          }
            return await _context.BookLanguage.ToListAsync();
        }

        // GET: api/BookLanguages/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookLanguage>> GetBookLanguage(string id)
        {
          if (_context.BookLanguage == null)
          {
              return NotFound();
          }
            var bookLanguage = await _context.BookLanguage.FindAsync(id);

            if (bookLanguage == null)
            {
                return NotFound();
            }

            return bookLanguage;
        }


        // POST: api/BookLanguages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<BookLanguage>> PostBookLanguage(BookLanguage bookLanguage)
        {
          if (_context.BookLanguage == null)
          {
              return Problem("Entity set 'LibraryAPIContext.BookLanguage'  is null.");
          }
            var language = await _context.Language.FindAsync(bookLanguage.LanguagesCode);
            var book = await _context.Book.FindAsync(bookLanguage.BooksId);

            if (language != null && language.IsDeleted || book != null && book.IsDeleted)
            {
                return BadRequest("The specified language and book are marked as deleted.");
            }
            _context.BookLanguage.Add(bookLanguage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookLanguageExists(bookLanguage.LanguagesCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookLanguage", new { id = bookLanguage.LanguagesCode }, bookLanguage);
        }

        // DELETE: api/BookLanguages/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLanguage(string id)
        {
            if (_context.BookLanguage == null)
            {
                return NotFound();
            }
            var bookLanguage = await _context.BookLanguage.FindAsync(id);
            if (bookLanguage == null)
            {
                return NotFound();
            }

            _context.BookLanguage.Remove(bookLanguage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookLanguageExists(string id)
        {
            return (_context.BookLanguage?.Any(e => e.LanguagesCode == id)).GetValueOrDefault();
        }
    }
}
