using Build_a_Web_API_for_an_E_Commorace.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Build_a_Web_API_for_an_E_Commorace
{
    internal class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //connection t database
            options.UseSqlServer(" Data Source=(localdb)\\MSSQLLOCALDB; Initial Catalog=Ecommerace; Integrated Security=true; TrustServerCertificate=True ");
        }


        //Registered classes
        public DbSet<User> Users{ get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderProducts> orderProducts { get; set; }

    }
}
