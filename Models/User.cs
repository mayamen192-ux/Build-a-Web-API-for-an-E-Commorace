using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Build_a_Web_API_for_an_E_Commorace.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [RegularExpression(
        @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain at least 8 characters, one uppercase letter, one lowercase letter, and one number."
    )]
        public string password { get; set; }
        [Required]
        [Phone]
        public string phone { get; set; }
        [Required]
        public string role { get; set; }
        [Required]
        public DateTime  createdAt{ get; set; }=DateTime.Now;

        //user can place many orders
          
        public virtual ICollection<Order> orders{ get; set; }

        //user can write many revirews

        public virtual ICollection<Review> reviews{ get; set; }




    }
}
