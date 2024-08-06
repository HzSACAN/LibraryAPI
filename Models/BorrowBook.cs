using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryAPI.Models
{
    public class BorrowBook
    {
        [Key]
        public long Id { get; set; }
        public string? MembersId { get; set; }
        public long BookCopiesId { get; set; }
        [Required]
        public DateTime BorrowTime { get; set; }
        [Required]
        public DateTime DueTime { get; set; }
        public DateTime? ReturnTime { get; set; }
        public bool IsReturn { get; set; } = false;
        public string? EmployeesId { get; set; }
        public bool IsDeleted { get; set; } =false;
        

        [ForeignKey(nameof(MembersId))]
        public Member? Member { get; set; }

        [ForeignKey(nameof(BookCopiesId))]  
        public BookCopy? BookCopy { get; set; }

        [ForeignKey(nameof(EmployeesId))]
        public Employee? Employees { get; set; }
    }
}
