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
using Microsoft.CodeAnalysis;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorBooksController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public AuthorBooksController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/AuthorBooks
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorBook>>> GetAuthorBook()
        {
          if (_context.AuthorBook == null)
          {
              return NotFound();
          }
            return await _context.AuthorBook.ToListAsync();
        }

        // GET: api/AuthorBooks/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(long id)
        {
          if (_context.AuthorBook == null)
          {
              return NotFound();
          }
            var authorBook = await _context.AuthorBook.FindAsync(id);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }


        // POST: api/AuthorBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<AuthorBook>> PostAuthorBook(AuthorBook authorBook)
        {
          if (_context.AuthorBook == null)
          {
              return Problem("Entity set 'LibraryAPIContext.AuthorBook'  is null.");
          }

            var author = await _context.Author.FindAsync(authorBook.AuthorsId);
            var book = await _context.Book.FindAsync(authorBook.BooksId);

            if ( author != null && author.IsDeleted || book != null && book.IsDeleted)
            {
                return BadRequest("The specified author and book are marked as deleted.");
            }
            _context.AuthorBook.Add(authorBook);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AuthorBookExists(authorBook.AuthorsId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAuthorBook", new { id = authorBook.AuthorsId }, authorBook);
        }

        // DELETE: api/AuthorBooks/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthorBook(long id)
        {
            if (_context.AuthorBook == null)
            {
                return NotFound();
            }
            var authorBook = await _context.AuthorBook.FindAsync(id);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBook.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorBookExists(long id)
        {
            return (_context.AuthorBook?.Any(e => e.AuthorsId == id)).GetValueOrDefault();
        }
    }
}
