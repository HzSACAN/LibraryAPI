using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class BookLanguage
    {
        public string LanguagesCode { get; set; } = "";

        public int BooksId { get; set; }

        [ForeignKey(nameof(LanguagesCode))]
        public Language? Language { get; set; }

        [ForeignKey(nameof(BooksId))]
        public Book? Book { get; set; } 
    }
}
