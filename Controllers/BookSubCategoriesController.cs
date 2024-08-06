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
    public class BookSubCategoriesController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public BookSubCategoriesController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/BookSubCategories
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSubCategory>>> GetSubCategoryBook()
        {
          if (_context.SubCategoryBook == null)
          {
              return NotFound();
          }
            return await _context.SubCategoryBook.ToListAsync();
        }

        // GET: api/BookSubCategories/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookSubCategory>> GetBookSubCategory(short id)
        {
          if (_context.SubCategoryBook == null)
          {
              return NotFound();
          }
            var bookSubCategory = await _context.SubCategoryBook.FindAsync(id);

            if (bookSubCategory == null)
            {
                return NotFound();
            }

            return bookSubCategory;
        }

        // POST: api/BookSubCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<BookSubCategory>> PostBookSubCategory(BookSubCategory bookSubCategory)
        {
          if (_context.SubCategoryBook == null)
          {
              return Problem("Entity set 'LibraryAPIContext.SubCategoryBook'  is null.");
          }
            var subCategory = await _context.SubCategory.FindAsync(bookSubCategory.SubCategoriesId);
            var book = await _context.Book.FindAsync(bookSubCategory.BooksId);

            if (subCategory != null && subCategory.IsDeleted || book != null && book.IsDeleted)
            {
                return BadRequest("The specified subcategory and book are marked as deleted.");
            }
            _context.SubCategoryBook.Add(bookSubCategory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookSubCategoryExists(bookSubCategory.SubCategoriesId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookSubCategory", new { id = bookSubCategory.SubCategoriesId }, bookSubCategory);
        }

        // DELETE: api/BookSubCategories/5
        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookSubCategory(short id)
        {
            if (_context.SubCategoryBook == null)
            {
                return NotFound();
            }
            var bookSubCategory = await _context.SubCategoryBook.FindAsync(id);
            if (bookSubCategory == null)
            {
                return NotFound();
            }

            _context.SubCategoryBook.Remove(bookSubCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookSubCategoryExists(short id)
        {
            return (_context.SubCategoryBook?.Any(e => e.SubCategoriesId == id)).GetValueOrDefault();
        }
    }
}
