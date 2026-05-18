using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Build_a_Web_API_for_an_E_Commorace.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int? Stock { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        [NotMapped]
        public decimal OverallRating
        {
            get
            {
                if (Reviews == null || !Reviews.Any())
                    return 0;

                return Math.Round((decimal)Reviews.Average(r => r.Rating), 1);
            }
        }
    }
}
