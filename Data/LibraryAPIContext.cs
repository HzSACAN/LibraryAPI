using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LibraryAPI.Data
{
    public class LibraryAPIContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryAPIContext (DbContextOptions<LibraryAPIContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AuthorBook>().HasKey(a => new { a.AuthorsId, a.BooksId });
            
            modelBuilder.Entity<BookLanguage>().HasKey(b => new { b.LanguagesCode, b.BooksId });

            modelBuilder.Entity<BookSubCategory>().HasKey(c => new { c.SubCategoriesId, c.BooksId });

            modelBuilder.Entity<Rating>().HasKey(d => new { d.BookId , d.MemberId  });
        }
        public DbSet<LibraryAPI.Models.Location> Location { get; set; } = default!;

        public DbSet<LibraryAPI.Models.Author>? Author { get; set; }

        public DbSet<LibraryAPI.Models.AuthorBook>? AuthorBook { get; set; }

        public DbSet<LibraryAPI.Models.Book>? Book { get; set; }

        public DbSet<LibraryAPI.Models.BookCopy>? BookCopy { get; set; }

        public DbSet<LibraryAPI.Models.BorrowBook>? BorrowBook { get; set; }

        public DbSet<LibraryAPI.Models.Category>? Category { get; set; }

        public DbSet<LibraryAPI.Models.Employee>? Employee { get; set; }

        public DbSet<LibraryAPI.Models.Language>? Language { get; set; }

        public DbSet<LibraryAPI.Models.BookLanguage>? BookLanguage { get; set; }

        public DbSet<LibraryAPI.Models.Member>? Member { get; set; }

        public DbSet<LibraryAPI.Models.Publisher>? Publisher { get; set; }

        public DbSet<LibraryAPI.Models.SubCategory>? SubCategory { get; set; }

        public DbSet<LibraryAPI.Models.BookSubCategory>? SubCategoryBook { get; set; }

        public DbSet<LibraryAPI.Models.Rating>? Rating { get; set; }
    }
}
