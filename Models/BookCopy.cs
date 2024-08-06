using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class BookCopy
    {
        [Key]
        public long Id { get; set; }
        public int BooksId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; }
    }
}
