using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly LibraryAPIContext _context;

        public RatingsController(LibraryAPIContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [Authorize(Roles ="Employee")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRating()
        {
          if (_context.Rating == null)
          {
              return NotFound();
          }
            return await _context.Rating.ToListAsync();
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Authorize'a gerek yok giriş yapan kişininin ve kitabın id si eşleşmezse değişiklik yapılmıyor zaten
        [HttpPut]
        public async Task<IActionResult> PutRating(long borrowedBookCopyId, Rating rating)
        {
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int targetBookId = (await _context.BookCopy!.FirstOrDefaultAsync(c => c.BooksId == borrowedBookCopyId))!.BooksId;

            Rating? targetRating = await _context.Rating!.FirstOrDefaultAsync(a => a.MemberId == loggedInUserId && a.BookId == targetBookId);

            if (targetRating == null)
            {
                return BadRequest();
            }

            if (rating.Rate < 0 || rating.Rate > 5)
            {
                return BadRequest();
            }

            targetRating.Rate=rating.Rate;

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(targetBookId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            List<Rating> ratings = await _context.Rating!.Where(c => c.BookId == targetBookId).ToListAsync();

            double averageRating = ratings.Average(d => d.Rate);

            Book? book = await _context.Book!.FindAsync(targetBookId);

            book!.Rating = averageRating;

            _context.Book.Update(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Ratings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // Employeeler de kitap ödünç alabiliyor sonuçta sisteme kayıtlı kişiler bunlar. Ama employee oy veremez. Kurgum böyle :D xD 
        [Authorize(Roles ="Member")]
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating, long borrowedBookCopyId)
        {
          if (_context.Rating == null)
          {
              return Problem("Entity set 'LibraryAPIContext.Rating'  is null.");
          }
            string loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            BorrowBook? searchBorrowedBook =await _context.BorrowBook!.FirstOrDefaultAsync(a => a.MembersId == loggedInUserId && a.BookCopiesId == borrowedBookCopyId)!;
            BookCopy? selectedBook= await _context.BookCopy!.FirstOrDefaultAsync(b => b.Id==borrowedBookCopyId);
            
            if (searchBorrowedBook == null || selectedBook == null) 
            {
                return BadRequest();
            }
            int booksId = selectedBook!.BooksId;

            rating.BookId = booksId;
            rating.MemberId=loggedInUserId;

            if (rating.Rate<0 || rating.Rate > 5)
            {
                return BadRequest();
            }

            _context.Rating.Add(rating);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RatingExists(rating.BookId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            List<Rating> ratings = await _context.Rating.Where(c => c.BookId == booksId).ToListAsync();

            double averageRating = ratings.Average(d => d.Rate);

            Book? book = await _context.Book!.FindAsync(booksId);

            book!.Rating = averageRating;

            _context.Book.Update(book);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtAction("GetRating", new { id = rating.BookId }, rating);
        }
        
        // Kimse verilen oyları silemez kullanıcı isterse gidip oyunu değiştirsin ama silemez

        private bool RatingExists(int id)
        {
            return (_context.Rating?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
