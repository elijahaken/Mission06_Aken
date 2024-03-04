using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mission06_Aken.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieId { get; set; }

        // Keep only one CategoryId property and make it nullable
        public int? CategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Year { get; set; }

        public string? Director { get; set; } // Nullable, assuming Director might not be specified

        public string? Rating { get; set; } // Nullable, assuming Rating might not be specified

        [Required]
        public bool Edited { get; set; } // Assuming 0 = false, 1 = true

        [Required]
        public bool CopiedToPlex { get; set; } // Assuming 0 = false, 1 = true

        public string? LentTo { get; set; } // Nullable, assuming LentTo might not be specified

        public string? Notes { get; set; } // Nullable, assuming Notes might not be specified

        // Navigation property to link back to Category
        // Removed 'required' keyword for compatibility and marked as nullable for EF Core
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
