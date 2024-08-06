using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace LibraryAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public long IdNumber { get; set; }
        public string Name { get; set; } = "";
        public string? MiddleName { get; set; }
        public string? FamilyName { get; set;}
        public string Address { get; set; } = "";
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegisterDate { get; set; }
        public byte Status { get; set; }
        [NotMapped]
        public string? PassWord { get; set; }
        [NotMapped]
        [Compare(nameof(PassWord))]
        public string? ConfirmPassword { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
