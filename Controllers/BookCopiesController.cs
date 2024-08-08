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
    public class BookCopiesController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public BookCopiesController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/BookCopies
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookCopy>>> GetBookCopy()
        {
          if (_context.BookCopy == null)
          {
              return NotFound();
          }
            var book = await _context.BookCopy.Include(b=> b.Book).ToListAsync();
            return book;
        }

        // GET: api/BookCopies/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookCopy>> GetBookCopy(long id)
        {
          if (_context.BookCopy == null)
          {
              return NotFound();
          }
            var bookCopy = await _context.BookCopy.Include(b=> b.Book).FirstOrDefaultAsync(a=>a.Id==id);

            if (bookCopy == null)
            {
                return NotFound();
            }

            return bookCopy;
        }

        // PUT: api/BookCopies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookCopy(long id, BookCopy bookCopy)
        {
            if (id != bookCopy.Id)
            {
                return BadRequest();
            }
            if (bookCopy.IsDeleted)
            {
                return BadRequest();
            }

            _context.Entry(bookCopy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyExists(id))
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

        // POST: api/BookCopies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<BookCopy>> PostBookCopy(BookCopy bookCopy)
        {
          if (_context.BookCopy == null)
          {
              return Problem("Entity set 'LibraryAPIContext.BookCopy'  is null.");
          }
            var book = await _context.Book.FindAsync(bookCopy.BooksId);

            if (book != null && book.IsDeleted)
            {
                return BadRequest("The specified book is marked as deleted.");
            }
            _context.BookCopy.Add(bookCopy);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookCopy", new { id = bookCopy.Id }, bookCopy);
        }

        // DELETE: api/BookCopies/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookCopy(long id)
        {
            if (_context.BookCopy == null)
            {
                return NotFound();
            }
            var bookCopy = await _context.BookCopy.FindAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }

            bookCopy.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookCopyExists(long id)
        {
            return (_context.BookCopy?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
