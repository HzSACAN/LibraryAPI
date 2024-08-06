using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class Rating
    {
        public int BookId { get; set; }
        public string? MemberId { get; set; }
        public byte Rate { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }

    }
}
