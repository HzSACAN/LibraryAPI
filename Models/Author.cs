using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryAPI.Models
{
    public class Author
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(800)]
        public string FullName { get; set; } = "";

        [Column(TypeName ="nvarchar(1000)")]
        public string? Biography { get; set; }

        [Range(-4000,2100)]
        public short BirthYear { get; set; }

        [Range(-4000,2100)]
        public short? DeathYear { get; set; }
        public bool IsDeleted { get; set; } = false;


        [JsonIgnore]
       public List<AuthorBook>? AuthorBooks { get; set; }
    }
}
