using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace LibraryAPI.Models
{
    public class BookSubCategory
    {
        public short SubCategoriesId { get; set; }
        public int BooksId { get; set; }

        [ForeignKey(nameof(SubCategoriesId))]
        public SubCategory? SubCategory { get; set; }

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }
    }
}
