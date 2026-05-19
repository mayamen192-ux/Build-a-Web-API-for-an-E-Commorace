using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Build_a_Web_API_for_an_E_Commorace.Models
{
    public class Order
    {
        [Key]
        public int order_Id { get; set; }

        // Foreign Key
        public int UserId { get; set; }

        // Navigation Property
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime? orderDate { get; set; }
      

        // many-many relationship with product via OrderProduct
        public ICollection<OrderProducts> OrderProducts { get; set; } = new List<OrderProducts>();

        // Calculated Property (Not stored in database)
        [Required]
        public decimal TotalAmount { get; set; }
        
     
       

      


    }
}
