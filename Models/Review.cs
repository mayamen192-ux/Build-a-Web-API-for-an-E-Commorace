using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Build_a_Web_API_for_an_E_Commorace.Models
{
    public class Review
    {
        [Key]
        public int Review_Id { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Foreign Key
        public int ProductId { get; set; }

        // Navigation Property
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, 5,
            ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
 
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; }
            = DateTime.Now;
    }
}