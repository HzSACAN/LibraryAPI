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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowBooksController : ControllerBase
    {
        private readonly LibraryAPIContext _context;
        private readonly UserManager<ApplicationUser>_userManager;

        public BorrowBooksController(LibraryAPIContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager= userManager;
        }
        // Member sadece kendine ait ödünç almaları çağırabilir
        [Authorize(Roles = "Member")]
        [HttpGet("Self")]
        public async Task<ActionResult<IEnumerable<BorrowBook>>> GetBorrowSelfBook()
        {
            if (_context.BorrowBook == null)
            {
                return NotFound();
            }
            string selfId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await _context.BorrowBook.Where(a => a.MembersId == selfId).ToListAsync() == null)
            {
                return NotFound();
            }

            return await _context.BorrowBook.Where(a => a.MembersId == selfId).ToListAsync();
        }

        // GET: api/BorrowBooks
        [Authorize(Roles ="Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BorrowBook>>> GetBorrowBook()
        {
          if (_context.BorrowBook == null)
          {
              return NotFound();
          }
            return await _context.BorrowBook.ToListAsync();
        }

        // PUT: api/BorrowBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Employee")]
        [HttpPut]
        public async Task<IActionResult> PutBorrowBook(long bookCopyId, BorrowBook borrowBook)
        {
            if (borrowBook.IsDeleted)
            {
                return BadRequest();
            }
            var selectedBookCopy = await _context.BorrowBook!.FirstOrDefaultAsync(b => b.BookCopiesId == bookCopyId && b.IsReturn== false);

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (selectedBookCopy==null)
            {
                return BadRequest();
            }

            selectedBookCopy.ReturnTime = borrowBook.ReturnTime;
            selectedBookCopy.IsReturn = true;
            selectedBookCopy.EmployeesId = loggedInUserId;

            _context.Entry(selectedBookCopy).State = EntityState.Modified;

            var bookCopy = await _context.BookCopy!.FindAsync(bookCopyId);

            bookCopy!.IsAvailable = true;

            _context.BookCopy.Update(bookCopy);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BorrowBookExists(selectedBookCopy.Id))
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

        // POST: api/BorrowBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles ="Employee")]
        [HttpPost]
        public async Task<ActionResult<BorrowBook>> PostBorrowBook(BorrowBook borrowBook, string userName)
        {
          if (_context.BorrowBook == null)
          {
              return Problem("Entity set 'LibraryAPIContext.BorrowBook'  is null.");
          }
            var user = await _userManager.FindByNameAsync(userName);
            var bookCopy = await _context.BookCopy!.FindAsync(borrowBook.BookCopiesId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (bookCopy == null)
            {
                return NotFound();
            }
            if (user.IsDeleted || bookCopy.IsDeleted)
            {
                return BadRequest("The specified book or member are marked as deleted.");
            }
            borrowBook.MembersId = user.Id;

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            borrowBook.EmployeesId = loggedInUserId;

            borrowBook.IsReturn = false;
            
            if (!bookCopy.IsAvailable)
            {
                return BadRequest();
            }
            bookCopy.IsAvailable = false;

            _context.BookCopy.Update(bookCopy);

            _context.BorrowBook.Add(borrowBook);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBorrowBook", new { id = borrowBook.Id }, borrowBook);
        }

        // DELETE: api/BorrowBooks/5
        [Authorize(Roles ="Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrowBook(long id)
        {
            if (_context.BorrowBook == null)
            {
                return NotFound();
            }
            var borrowBook = await _context.BorrowBook.FindAsync(id);
            if (borrowBook == null)
            {
                return NotFound();
            }

            borrowBook.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BorrowBookExists(long id)
        {
            return (_context.BorrowBook?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
