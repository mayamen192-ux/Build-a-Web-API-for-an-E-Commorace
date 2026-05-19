using Build_a_Web_API_for_an_E_Commorace.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Build_a_Web_API_for_an_E_Commorace
{
    internal class Program
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        static User loggedInUser = null;

        // Defined Function
        static  public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        static  public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        static public void RegisterUser()
        {
            Console.WriteLine("Enter User Name");
            string name = Console.ReadLine();

            Console.WriteLine("Enter User Email");
            string email = Console.ReadLine();

            Console.WriteLine("Enter Password");
            string password = Console.ReadLine();

            Console.WriteLine("Enter User Phone Number");
            string phone = Console.ReadLine();

            Console.WriteLine("Enter User Role");
            string role = Console.ReadLine();

            // Empty validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                Console.WriteLine("All fields are required!");
                return;
            }

            // EMAIL VALIDATION
            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format!");
                return;
            }

            // UNIQUE EMAIL
            var existingUser = db.Users.FirstOrDefault(u => u.email == email);

            if (existingUser != null)
            {
                Console.WriteLine("Email already exists.");
                return;
            }

            // PASSWORD VALIDATION
            string passwordPattern =
                @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$";

            if (!Regex.IsMatch(password, passwordPattern))
            {
                Console.WriteLine(
                    "Password must contain:\n" +
                    "- At least 8 characters\n" +
                    "- One uppercase letter\n" +
                    "- One lowercase letter\n" +
                    "- One number"
                );
                return;
            }

            // PHONE VALIDATION
            var phoneAttribute = new PhoneAttribute();

            if (!phoneAttribute.IsValid(phone))
            {
                Console.WriteLine("Invalid phone number.");
                return;
            }

            var user = new User
            {
                name = name,
                email = email,
                password = HashPassword(password), // ✔ HASHED
                phone = phone,
                role = role,
                createdAt = DateTime.Now,
                IsActive = true
            };

            db.Users.Add(user);

            db.SaveChanges();

            Console.WriteLine("User Registered Successfully!");
        }
        static public void LoginUser()
        {
            try
            {
                Console.WriteLine("Enter Email:");
                string emailLogin = Console.ReadLine();

                Console.WriteLine("Enter Password:");
                string PasswordLogin = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(emailLogin) ||
                    string.IsNullOrWhiteSpace(PasswordLogin))
                {
                    Console.WriteLine("Email and Password are required!");
                    return;
                }

                string hashedPassword = HashPassword(PasswordLogin);

                var user = db.Users.FirstOrDefault(
                    u => u.email == emailLogin &&
                         u.password == hashedPassword
                );

                if (user == null)
                {
                    Console.WriteLine("Invalid email or password!");
                    return;
                }

                if (!user.IsActive)
                {
                    Console.WriteLine("Your account is inactive.");
                    return;
                }

                loggedInUser = user;

                Console.WriteLine("Login Successful!");
                Console.WriteLine("Welcome " + user.name);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        static  public void GetUserDetails()
        {
            Console.WriteLine("Enter User ID:");
            int id = int.Parse(Console.ReadLine());

            User user1 = db.Users.Find(id);

            if (user1 == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            Console.WriteLine(
                "User Name: " + user1.name +
                " , User Email: " + user1.email +
                " , User Phone: " + user1.phone +
                " , User Role: " + user1.role +
                " , Date: " + user1.createdAt
            );

            Console.WriteLine("Get user details...");
        }
        static public void AddProduct()
        {
            Console.WriteLine("Enter Admin Role:");
            string role = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(role) || role.ToLower() != "admin")
            {
                Console.WriteLine("Access denied. Admins only!");
                return;
            }

            Console.WriteLine("Enter Product Name:");
            string ProName = Console.ReadLine();

            Console.WriteLine("Enter Product Description:");
            string ProDescription = Console.ReadLine();

            Console.WriteLine("Enter Product Price:");

            if (!decimal.TryParse(Console.ReadLine(), out decimal ProPrice))
            {
                Console.WriteLine("Invalid price!");
                return;
            }

            //  PRICE VALIDATION
            if (ProPrice <= 0)
            {
                Console.WriteLine("Product price must be greater than zero.");
                return;
            }

            Console.WriteLine("Enter Product Stock:");

            if (!int.TryParse(Console.ReadLine(), out int ProStock))
            {
                Console.WriteLine("Invalid stock value!");
                return;
            }

            // ✔ STOCK VALIDATION
            if (ProStock < 0)
            {
                Console.WriteLine("Stock cannot be negative.");
                return;
            }

            db.Products.Add(new Product
            {
                Name = ProName,
                Description = ProDescription,
                Price = ProPrice,
                Stock = ProStock
            });

            db.SaveChanges();

            Console.WriteLine("Added product successfully.");
        }
        static public void UpdateProduct()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                return;
            }

            if (loggedInUser.role.ToLower() != "admin")
            {
                Console.WriteLine("Access denied! Admin only.");
                return;
            }

            Console.WriteLine("Enter Product Id:");

            if (!int.TryParse(Console.ReadLine(), out int proID))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }

            Product pro = db.Products.Find(proID);

            if (pro == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine("Enter Product Name:");
            string proName = Console.ReadLine();

            Console.WriteLine("Enter Description:");
            string description = Console.ReadLine();

            Console.WriteLine("Enter Product Price:");

            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            // PRICE VALIDATION
            if (price <= 0)
            {
                Console.WriteLine("Product price must be greater than zero.");
                return;
            }

            Console.WriteLine("Enter Product Stock:");

            if (!int.TryParse(Console.ReadLine(), out int pStock))
            {
                Console.WriteLine("Invalid stock.");
                return;
            }

            // STOCK VALIDATION
            if (pStock < 0)
            {
                Console.WriteLine("Stock cannot be negative.");
                return;
            }

            pro.Name = proName;
            pro.Description = description;
            pro.Price = price;
            pro.Stock = pStock;

            db.Products.Update(pro);
            db.SaveChanges();

            Console.WriteLine("Product updated successfully!");
        }
        static  public void FilterProducts()
        {
            Console.WriteLine("Enter Product Name Filter (or press Enter to skip):");
            string nameFilter = Console.ReadLine();

            Console.WriteLine("Enter Min Price:");
            
            decimal minPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter Max Price:");
            decimal maxPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter Page Number:");
            int pageNumber = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Page Size:");
            int pageSize = int.Parse(Console.ReadLine());

            // Filtering
            var query = db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(p => p.Name.Contains(nameFilter));
            }

            query = query.Where
            (
                p => p.Price >= minPrice && p.Price <= maxPrice
            );
            if (minPrice < 0 || maxPrice < 0)
            {
                Console.WriteLine("Price cannot be negative.");
                return;
            }

            if (minPrice > maxPrice)
            {
                Console.WriteLine("Min price cannot be greater than max price.");
                return;
            }

            // Pagination
            var products = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Display
            foreach (var p in products)
            {
                Console.WriteLine(
                    "Product Name: " + p.Name +
                    ", Description: " + p.Description +
                    ", Price: " + p.Price +
                    ", Stock: " + p.Stock +
                    ", Overall Rating: " + p.OverallRating
                );
            }
        }
        static  public void GetProductDetails()
        {
            if (!int.TryParse(Console.ReadLine(), out int id2))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }

            Product Pp = db.Products.Find(id2);

            if (Pp == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine("Product details...");
            Console.WriteLine(
                "Product Name:" + Pp.Name +
                ", Description:" + Pp.Description +
                " , Price:" + Pp.Price +
                " , Stock:" + Pp.Stock +
                " , Overall Rating:" + Pp.OverallRating
            );
        }
        public static void PlaceNewOrder()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("You must login first.");
                return;
            }

            var order = new Order
            {
                UserId = loggedInUser.Id,
                orderDate = DateTime.Now,
                TotalAmount = 0
            };

            db.Orders.Add(order);
            db.SaveChanges();

            var products = db.Products.ToList();

            while (true)
            {
                Console.WriteLine("\n=== Available Products ===");

                foreach (var p in products)
                {
                    Console.WriteLine(
                        $"ID: {p.Id} | Name: {p.Name} | Price: {p.Price} | Stock: {p.Stock}"
                    );
                }

                Console.Write("Enter Product ID (0 to finish): ");

                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (productId == 0)
                    break;

                var product = products.FirstOrDefault(p => p.Id == productId);

                if (product == null)
                {
                    Console.WriteLine("Invalid product.");
                    continue;
                }

                Console.Write("Enter Quantity: ");

                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }

                if (product.Stock < qty)
                {
                    Console.WriteLine("Not enough stock.");
                    continue;
                }

                var existingOrderProduct = db.orderProducts
                    .FirstOrDefault(op =>
                        op.OrderId == order.order_Id &&
                        op.ProductId == product.Id);

                if (existingOrderProduct != null)
                {
                    existingOrderProduct.Quantity += qty;
                }
                else
                {
                    db.orderProducts.Add(new OrderProducts
                    {
                        OrderId = order.order_Id,
                        ProductId = product.Id,
                        Quantity = qty
                    });
                }

                product.Stock -= qty;

                Console.WriteLine("Product added.");
            }


            // Prevent empty order
            // Save products first
            db.SaveChanges();

            // Prevent empty order
            db.SaveChanges();

            // Prevent empty order
            bool hasProducts = db.orderProducts
                                 .Any(op => op.OrderId == order.order_Id);

            if (!hasProducts)
            {
                db.Orders.Remove(order);
                db.SaveChanges();

                Console.WriteLine("Order cancelled. No products selected.");
                return;
            }

            decimal total = db.orderProducts
                     .Where(op => op.OrderId == order.order_Id)
                         .Join(
                         db.Products,
                        op => op.ProductId,
                        p => p.Id,
                      (op, p) => p.Price * op.Quantity
    )
    .Sum();

            // Update order total
            order.TotalAmount = total;

            // Tell EF that Order changed
            db.Orders.Update(order);


            db.SaveChanges();

            Console.WriteLine("===== ORDER COMPLETED =====");
            Console.WriteLine($"Order ID: {order.order_Id}");
            Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
        }
        public static void GetUserOrders()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("No user is logged in!");
                return;
            }

            var orders = db.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Where(o => o.UserId == loggedInUser.Id)
                .OrderByDescending(o => o.orderDate)
                .ToList();

            Console.WriteLine("=== My Orders ===");

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders yet.");
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"Order: {order.order_Id}");
                Console.WriteLine($"Total: {order.TotalAmount:C}");
                Console.WriteLine($"Date: {order.orderDate:dd/MM/yyyy}");
                Console.WriteLine($"Products:");

                foreach (var op in order.OrderProducts)
                {
                    Console.WriteLine($" - {op.Product.Name} x {op.Quantity} | {op.Product.Price:C} each");
                }
            }
        }
        public static void OrderDetail()
        {
            Console.WriteLine("Enter order ID: ");
            int orderID = int.Parse(Console.ReadLine());

            Order orders = db.Orders.Find(orderID);

            if (orders == null)
            {
                Console.WriteLine("Order not found!");
                return;
            }

            Console.WriteLine("Order Date: " + orders.orderDate);
            Console.WriteLine("Order total amount: " + orders.TotalAmount);
        }
        public static void AddReview()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("You must login first.");
                return;
            }

            Console.Write("Enter product ID: ");

            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }

            var product = db.Products
                            .Include(p => p.Reviews)
                            .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                Console.WriteLine($"Product with ID {productId} not found.");
                return;
            }

            // Prevent duplicate review
            var existingReview = db.Reviews.FirstOrDefault(r =>
                r.UserId == loggedInUser.Id &&
                r.ProductId == productId);

            if (existingReview != null)
            {
                Console.WriteLine("You have already reviewed this product.");
                return;
            }

            Console.Write("Enter Rating (1-5): ");

            if (!int.TryParse(Console.ReadLine(), out int rate) ||
                rate < 1 || rate > 5)
            {
                Console.WriteLine("Invalid rating. Must be 1 to 5.");
                return;
            }

            Console.Write("Enter comment: ");
            string comment = Console.ReadLine()?.Trim();

            var review = new Review
            {
                UserId = loggedInUser.Id,
                ProductId = productId,
                Rating = rate,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            db.Reviews.Add(review);
            db.SaveChanges();

            // Reload reviews to get updated average
            db.Entry(product)
              .Collection(p => p.Reviews)
              .Load();

            Console.WriteLine("Review added successfully!");
            Console.WriteLine($"New Product Rating: {product.OverallRating:F1}");
        }
        public static void GetAllReview()
        {
            Console.WriteLine("Enter product ID : ");
            int id = int.Parse(Console.ReadLine().Trim());

            Product product = db.Products.Include(p => p.Reviews)
                                           .FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                int page = 1;
                int pageSize = 5;

                var review = product.Reviews.OrderByDescending(r => r.ReviewDate)
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();

                foreach (var Review in review)
                {
                    Console.WriteLine("Reviews for " + product.Name + ": ");
                    Console.WriteLine("Rating: " + Review.Rating);
                    Console.WriteLine("Comment: " + Review.Comment);
                    Console.WriteLine("Date of review: " + Review.ReviewDate);
                }
            }

            else
            {
                Console.WriteLine("Product with " + id + " not found.");
            }
        }
        public static void EditReview()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("You must login first.");
                return;
            }

            var myReviews = db.Reviews
                .Where(r => r.UserId == loggedInUser.Id)
                .ToList();

            if (!myReviews.Any())
            {
                Console.WriteLine("No reviews found.");
                return;
            }

            Console.WriteLine("=== Your Reviews ===");

            foreach (var r in myReviews)
            {
                Console.WriteLine($"ID: {r.Review_Id} | Product: {r.ProductId} | Rating: {r.Rating}");
                Console.WriteLine($"Comment: {r.Comment}");
                Console.WriteLine("--------------------------------");
            }

            Console.Write("Enter Review ID to edit/delete: ");

            if (!int.TryParse(Console.ReadLine(), out int revId))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            var review = db.Reviews
                .FirstOrDefault(r => r.Review_Id == revId && r.UserId == loggedInUser.Id);

            if (review == null)
            {
                Console.WriteLine("Review not found.");
                return;
            }

            Console.WriteLine("1. Edit Review");
            Console.WriteLine("2. Delete Review");
            Console.Write("Choose option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            if (choice == 1)
            {
                Console.Write("Enter new comment: ");
                string newComm = Console.ReadLine();

                Console.Write("Enter new rating (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out int newRate) || newRate < 1 || newRate > 5)
                {
                    Console.WriteLine("Invalid rating.");
                    return;
                }

                review.Comment = newComm;
                review.Rating = newRate;

                db.SaveChanges();

                Console.WriteLine("Review updated successfully.");
            }
            else if (choice == 2)
            {
                Console.WriteLine("Are you sure you want to delete this review? (y/n)");
                string confirm = Console.ReadLine()?.Trim().ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    db.Reviews.Remove(review);
                    db.SaveChanges();

                    Console.WriteLine("Review deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Delete cancelled.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }

            Console.ReadKey();
        }
        static public void LoginNewUser()
        {
            Console.WriteLine("Enter Email:");
            string emailLogin = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            string PasswordLogin = Console.ReadLine();

            var user = db.Users.FirstOrDefault(u =>
                u.email == emailLogin &&
                u.password == PasswordLogin);

            if (user == null)
            {
                Console.WriteLine("Invalid email or password!");
                return;
            }

            loggedInUser = user;
         

            Console.WriteLine("Login Successful!");
            Console.WriteLine("Welcome " + user.name);
        }
        public static bool Logout()
        {
            Console.WriteLine("Are you sure you want to logout? (yes/no): ");
            string confirmLogout = Console.ReadLine() ?? string.Empty;

            if (confirmLogout == "yes")
            {
                Console.WriteLine("Loging system...");
                Console.WriteLine("Thank you for using E-Commerce System!");
                return true;
            }
            else
            {
                Console.WriteLine("Exit cancelled. Returning to main menu...");
                return false;
            }
        }

        public static void Menu()
        {
            bool logout = false;

            while (!logout)
            {
                Console.WriteLine("\nWelcome to the E-Commerce System:");
                Console.WriteLine("--------------------------------");
             
                Console.WriteLine("1. User API");
                Console.WriteLine("2. Product API");
                Console.WriteLine("3. Order API");
                Console.WriteLine("4. Review API");
                Console.WriteLine("5. User Log-out");

                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid number!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        UserMenu();
                        break;

                    case 2:
                        ProductMenu();
                        break;

                    case 3:
                        OrderMenu();
                        break;

                    case 4:
                        ReviewMenu();
                        break;

                    case 5:
                        logout = Logout();
                        break;

                   

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                Console.WriteLine("\nPress any key...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // ================= USER MENU =================
        static void UserMenu()
        {
            Console.WriteLine("\nUser API:");
       
            Console.WriteLine("1. Get user details by ID");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
               

                case 1:
                    GetUserDetails();
                    break;
            }
        }

        // ================= PRODUCT MENU =================
        static void ProductMenu()
        {
            Console.WriteLine("\nProduct API:");
            Console.WriteLine("1. Add new product (Admin only)");
            Console.WriteLine("2. Update product (Admin only) or Delete product ");
            Console.WriteLine("3. Get products (pagination/filter)");
            Console.WriteLine("4. Get product by ID");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddProduct();
                    break;

                case 2:

                    UpdateProduct();
                    break;

                case 3:
                    FilterProducts();

                    break;

                case 4:

                    GetProductDetails();

                    break;
            }
        }

        // ================= ORDER MENU =================
        static void OrderMenu()
        {
            Console.WriteLine("\nOrder API:");
            Console.WriteLine("1. Place new order");
            Console.WriteLine("2. Get all orders for user");
            Console.WriteLine("3. Get order by ID");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    PlaceNewOrder();
                    break;

                case 2:
                    GetUserOrders();
                    break;

                case 3:
                    OrderDetail();


                    break;
            }
        }

        // ================= REVIEW MENU =================
        static void ReviewMenu()
        {
            Console.WriteLine("\nReview API:");
            Console.WriteLine("1. Add review");
            Console.WriteLine("2. Get reviews for product (pagination)");
            Console.WriteLine("3. Update review or Delete review");
         

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddReview();
                    break;

                case 2:
                    GetAllReview();
                    break;

                case 3:
                    EditReview();
                            break;

                    
            
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        
        
            }
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== E‑Commerce System =====");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");

                Console.Write("Choose option: ");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        RegisterUser();
                        break;

                    case 2:
                        LoginUser();

                        Menu();
                        break;

                    case 3:
                        exit = true;

                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
    
