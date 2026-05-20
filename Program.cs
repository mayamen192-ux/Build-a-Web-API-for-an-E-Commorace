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

        // ─────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────

        static public bool IsValidEmail(string email)
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

        static public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // ─────────────────────────────────────────────
        //  USER
        // ─────────────────────────────────────────────

        static public void RegisterUser()
        {
            try
            {
                Console.WriteLine("Enter User Name:");
                string name = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Enter User Email:");
                string email = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Enter Password:");
                string password = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Enter User Phone Number:");
                string phone = Console.ReadLine() ?? string.Empty;

                Console.WriteLine("Enter User Role:");
                string role = Console.ReadLine() ?? string.Empty;

                // FIX 1: All validations BEFORE hitting the database
                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(phone) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(role))
                {
                    Console.WriteLine("All fields are required!");
                    Console.ReadKey();
                    return;
                }

                if (!IsValidEmail(email))
                {
                    Console.WriteLine("Invalid email format!");
                    Console.ReadKey();
                    return;
                }

                string passwordPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$";
                if (!Regex.IsMatch(password, passwordPattern))
                {
                    Console.WriteLine(
                        "Password must contain:\n" +
                        "- At least 8 characters\n" +
                        "- One uppercase letter\n" +
                        "- One lowercase letter\n" +
                        "- One number"
                    );
                    Console.ReadKey();
                    return;
                }

                var phoneAttribute = new PhoneAttribute();
                if (!phoneAttribute.IsValid(phone))
                {
                    Console.WriteLine("Invalid phone number.");
                    Console.ReadKey();
                    return;
                }

                var existingUser = db.Users.FirstOrDefault(u => u.email == email);
                if (existingUser != null)
                {
                    Console.WriteLine("Email already exists.");
                    Console.ReadKey();
                    return;
                }

                var user = new User
                {
                    name = name,
                    email = email,
                    password = HashPassword(password),   // stored as hash
                    phone = phone,
                    role = role,
                    createdAt = DateTime.Now,
                    IsActive = true
                };

                db.Users.Add(user);
                db.SaveChanges();

                Console.WriteLine("User Registered Successfully!");
                // FIX 2: Pause so the message is visible before Console.Clear()
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message: " + ex.Message);
                Console.WriteLine("Inner Error: " + ex.InnerException?.Message);
                Console.ReadKey();  // FIX 3: pause on errors too
            }
        }

        static public void LoginUser()
        {
            try
            {
                Console.WriteLine("Enter Email:");
                string emailLogin = Console.ReadLine();

                Console.WriteLine("Enter Password:");
                string passwordLogin = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(emailLogin) ||
                    string.IsNullOrWhiteSpace(passwordLogin))
                {
                    Console.WriteLine("Email and Password are required!");
                    return;
                }

                string hashedPassword = HashPassword(passwordLogin);

                var user = db.Users.FirstOrDefault(
                    u => u.email == emailLogin &&
                         u.password == hashedPassword   // compare hash → hash
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

        static public void GetUserDetails()
        {
            Console.WriteLine("Enter User ID:");

            // FIX 4: safe parse instead of int.Parse (crash on bad input)
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            User user1 = db.Users.Find(id);

            if (user1 == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            Console.WriteLine(
                "User Name: " + user1.name +
                " | Email: " + user1.email +
                " | Phone: " + user1.phone +
                " | Role: " + user1.role +
                " | Created: " + user1.createdAt
            );
        }

        // ─────────────────────────────────────────────
        //  PRODUCTS
        // ─────────────────────────────────────────────

        static public void AddProduct()
        {
            //   unauthenticated users
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey();
                return;
            }

            //  non-admin logged-in users
            if (loggedInUser.role.ToLower() != "admin")
            {
                Console.WriteLine("Access denied! Admins only.");
                Console.ReadKey();
                return;
            }

            //  product name
            Console.WriteLine("Enter Product Name:");
            string proName = Console.ReadLine();

            //  product description
            Console.WriteLine("Enter Product Description:");
            string proDescription = Console.ReadLine();

            // no crash on bad price input
            Console.WriteLine("Enter Product Price:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal proPrice))
            {
                Console.WriteLine("Invalid price!");
                Console.ReadKey();
                return;
            }

            //  price must be greater than zero
            if (proPrice <= 0)
            {
                Console.WriteLine("Product price must be greater than zero.");
                Console.ReadKey();
                return;
            }

            // no crash on bad stock input
            Console.WriteLine("Enter Product Stock:");
            if (!int.TryParse(Console.ReadLine(), out int proStock))
            {
                Console.WriteLine("Invalid stock value!");
                Console.ReadKey();
                return;
            }

            // stock cannot be negative
            if (proStock < 0)
            {
                Console.WriteLine("Stock cannot be negative.");
                Console.ReadKey();
                return;
            }

            // persists the new product to the database
            db.Products.Add(new Product
            {
                Name = proName,
                Description = proDescription,
                Price = proPrice,
                Stock = proStock
            });

            db.SaveChanges();

            // confirms success and pauses so message is visible
            Console.WriteLine("Product added successfully.");
            Console.ReadKey();
        }

        static public void UpdateProduct()
        {
            //  blocks unauthenticated users
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            //  blocks non-admin logged-in users
            if (loggedInUser.role.ToLower() != "admin")
            {
                Console.WriteLine("Access denied! Admin only.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            Console.WriteLine("Enter Product Id:");
            if (!int.TryParse(Console.ReadLine(), out int proID))
            {
                Console.WriteLine("Invalid product ID.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            Product pro = db.Products.Find(proID);
            if (pro == null)
            {
                Console.WriteLine("Product not found!");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            // Show current values so admin knows what they are changing
            Console.WriteLine("================================");
            Console.WriteLine("Current Product Details:");
            Console.WriteLine($"  Name        : {pro.Name}");
            Console.WriteLine($"  Description : {pro.Description}");
            Console.WriteLine($"  Price       : {pro.Price:C}");
            Console.WriteLine($"  Stock       : {pro.Stock}");
            Console.WriteLine("================================");

            Console.WriteLine("Enter New Product Name:");
            string proName = Console.ReadLine();

            // name cannot be empty
            if (string.IsNullOrWhiteSpace(proName))
            {
                Console.WriteLine("Product name cannot be empty.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Enter New Description:");
            string description = Console.ReadLine();

            //  no crash on bad price input
            Console.WriteLine("Enter New Product Price:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                Console.WriteLine("Invalid price.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            // price must be greater than zero
            if (price <= 0)
            {
                Console.WriteLine("Product price must be greater than zero.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            // no crash on bad stock input
            Console.WriteLine("Enter New Product Stock:");
            if (!int.TryParse(Console.ReadLine(), out int pStock))
            {
                Console.WriteLine("Invalid stock.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            // stock cannot be negative
            if (pStock < 0)
            {
                Console.WriteLine("Stock cannot be negative.");
                Console.ReadKey(); // FIX: was missing
                return;
            }

            pro.Name = proName;
            pro.Description = description;
            pro.Price = price;
            pro.Stock = pStock;

            db.Products.Update(pro);
            db.SaveChanges();

            // confirms what was updated
            Console.WriteLine("================================");
            Console.WriteLine("Product updated successfully!");
            Console.WriteLine("Updated Details:");
            Console.WriteLine($"  Name        : {pro.Name}");
            Console.WriteLine($"  Description : {pro.Description}");
            Console.WriteLine($"  Price       : {pro.Price:C}");
            Console.WriteLine($"  Stock       : {pro.Stock}");
            Console.WriteLine("================================");
            Console.ReadKey(); // ✔ keeps success message visible
        }

        static public void FilterProducts()
        {
            Console.WriteLine("Enter Product Name Filter (or press Enter to skip):");
            string nameFilter = Console.ReadLine();

            // FIX 6: safe parse for min/max price
            Console.WriteLine("Enter Min Price:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
            {
                Console.WriteLine("Invalid min price.");
                return;
            }

            Console.WriteLine("Enter Max Price:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
            {
                Console.WriteLine("Invalid max price.");
                return;
            }

            // FIX 7: validate BEFORE querying (was after in original)
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

            Console.WriteLine("Enter Page Number:");
            if (!int.TryParse(Console.ReadLine(), out int pageNumber) || pageNumber < 1)
            {
                Console.WriteLine("Invalid page number.");
                return;
            }

            Console.WriteLine("Enter Page Size:");
            if (!int.TryParse(Console.ReadLine(), out int pageSize) || pageSize < 1)
            {
                Console.WriteLine("Invalid page size.");
                return;
            }

            var query = db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(p => p.Name.Contains(nameFilter));

            query = query.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            var products = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!products.Any())
            {
                Console.WriteLine("No products found.");
                return;
            }

            foreach (var p in products)
            {
                Console.WriteLine(
                    "Name: " + p.Name +
                    " | Description: " + p.Description +
                    " | Price: " + p.Price +
                    " | Stock: " + p.Stock +
                    " | Rating: " + p.OverallRating
                );
            }
        }

        static public void GetProductDetails()
        {
            // FIX 8: added prompt — original had no prompt before ReadLine
            Console.WriteLine("Enter Product ID:");
            if (!int.TryParse(Console.ReadLine(), out int id2))
            {
                Console.WriteLine("Invalid product ID.");
                return;
            }

            Product pp = db.Products.Find(id2);
            if (pp == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.WriteLine(
                "Name: " + pp.Name +
                " | Desc: " + pp.Description +
                " | Price: " + pp.Price +
                " | Stock: " + pp.Stock +
                " | Rating: " + pp.OverallRating
            );
        }

        // ─────────────────────────────────────────────
        //  ORDERS
        // ─────────────────────────────────────────────

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

            // FIX 9: refresh stock from DB each loop iteration to stay accurate
            while (true)
            {
                var products = db.Products.ToList();

                Console.WriteLine("\n=== Available Products ===");
                foreach (var p in products)
                    Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Price: {p.Price:C} | Stock: {p.Stock}");

                Console.Write("Enter Product ID (0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (productId == 0) break;

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
                    existingOrderProduct.Quantity += qty;
                else
                    db.orderProducts.Add(new OrderProducts
                    {
                        OrderId = order.order_Id,
                        ProductId = product.Id,
                        Quantity = qty
                    });

                product.Stock -= qty;
                db.SaveChanges();   // FIX 10: single SaveChanges per iteration (no duplicate calls)
                Console.WriteLine("Product added.");
            }

            bool hasProducts = db.orderProducts.Any(op => op.OrderId == order.order_Id);

            if (!hasProducts)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                Console.WriteLine("Order cancelled. No products selected.");
                return;
            }

            decimal total = db.orderProducts
                .Where(op => op.OrderId == order.order_Id)
                .Join(db.Products,
                      op => op.ProductId,
                      p => p.Id,
                      (op, p) => p.Price * op.Quantity)
                .Sum();

            order.TotalAmount = total;
            db.Orders.Update(order);
            db.SaveChanges();

            Console.WriteLine("===== ORDER COMPLETED =====");
            Console.WriteLine($"Order ID:     {order.order_Id}");
            Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
        }

        public static void GetUserOrders()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey();
                return;
            }

            var orders = db.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Where(o => o.UserId == loggedInUser.Id)
                .OrderByDescending(o => o.orderDate)
                .ToList();

            Console.WriteLine($"=== Orders for {loggedInUser.name} ===");

            if (!orders.Any())
            {
                Console.WriteLine("No orders yet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Total Orders Found: {orders.Count}");
            Console.WriteLine("================================");

            foreach (var order in orders)
            {
                Console.WriteLine($"\nOrder ID:  {order.order_Id}");
                Console.WriteLine($"Date:      {order.orderDate:dd/MM/yyyy}");
                Console.WriteLine($"Total:     {order.TotalAmount:C}");
                Console.WriteLine("Products:");

                foreach (var op in order.OrderProducts)
                {
                    decimal lineTotal = op.Product.Price * op.Quantity;
                    Console.WriteLine($"  - {op.Product.Name}");
                    Console.WriteLine($"    Quantity : {op.Quantity}");
                    Console.WriteLine($"    Price    : {op.Product.Price:C} each");
                    Console.WriteLine($"    Subtotal : {lineTotal:C}");
                }

                Console.WriteLine("--------------------------------");
            }

            Console.ReadKey(); // ← keeps output visible before screen clears
        }

        public static void OrderDetail()
        {
            // FIX 1: authentication check — requirement says "authenticated users only"
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Enter order ID:");
            if (!int.TryParse(Console.ReadLine(), out int orderID))
            {
                Console.WriteLine("Invalid order ID.");
                Console.ReadKey();
                return;
            }

            // FIX 2: Include products so we can display the full order details
            Order order = db.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.order_Id == orderID);

            if (order == null)
            {
                Console.WriteLine("Order not found!");
                Console.ReadKey();
                return;
            }

            // FIX 3: users can only view their OWN orders, not other users' orders
            if (order.UserId != loggedInUser.Id)
            {
                Console.WriteLine("Access denied! This order does not belong to you.");
                Console.ReadKey();
                return;
            }

            // Display full order details
            Console.WriteLine("================================");
            Console.WriteLine($"Order ID:    {order.order_Id}");
            Console.WriteLine($"Date:        {order.orderDate:dd/MM/yyyy}");
            Console.WriteLine($"Total:       {order.TotalAmount:C}");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Products:");

            foreach (var op in order.OrderProducts)
            {
                decimal lineTotal = op.Product.Price * op.Quantity;
                Console.WriteLine($"  - {op.Product.Name}");
                Console.WriteLine($"    Quantity : {op.Quantity}");
                Console.WriteLine($"    Price    : {op.Product.Price:C} each");
                Console.WriteLine($"    Subtotal : {lineTotal:C}");
            }

            Console.WriteLine("================================");
            Console.ReadKey(); // ← keeps output visible before screen clears
        }
        // ─────────────────────────────────────────────
        //  REVIEWS
        // ─────────────────────────────────────────────

        public static void AddReview()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Invalid product ID.");
                Console.ReadKey();
                return;
            }

            var product = db.Products
                            .Include(p => p.Reviews)
                            .FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                Console.WriteLine($"Product with ID {productId} not found.");
                Console.ReadKey();
                return;
            }

            // FIX: requirement says product must be part of a previous order for this user
            bool hasPurchased = db.Orders
                .Where(o => o.UserId == loggedInUser.Id)
                .Any(o => o.OrderProducts.Any(op => op.ProductId == productId));

            if (!hasPurchased)
            {
                Console.WriteLine("You can only review products you have ordered before.");
                Console.ReadKey();
                return;
            }

            var existingReview = db.Reviews.FirstOrDefault(r =>
                r.UserId == loggedInUser.Id &&
                r.ProductId == productId);

            if (existingReview != null)
            {
                Console.WriteLine("You have already reviewed this product.");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Rating (1-5): ");
            if (!int.TryParse(Console.ReadLine(), out int rate) || rate < 1 || rate > 5)
            {
                Console.WriteLine("Invalid rating. Must be 1 to 5.");
                Console.ReadKey();
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

            // Reload reviews to recalculate the average rating
            db.Entry(product).Collection(p => p.Reviews).Load();

            Console.WriteLine("================================");
            Console.WriteLine("Review added successfully!");
            Console.WriteLine($"Product:    {product.Name}");
            Console.WriteLine($"Your Rating: {rate}/5");
            Console.WriteLine($"New Overall Rating: {product.OverallRating:F1}/5");
            Console.WriteLine("================================");
            Console.ReadKey(); // ← keeps output visible before screen clears
        }

        public static void GetAllReview()
        {
            Console.WriteLine("Enter product ID:");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int id))
            {
                Console.WriteLine("Invalid product ID.");
                Console.ReadKey();
                return;
            }

            Product product = db.Products
                                 .Include(p => p.Reviews)
                                 .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                Console.ReadKey();
                return;
            }

            if (!product.Reviews.Any())
            {
                Console.WriteLine($"No reviews yet for {product.Name}.");
                Console.ReadKey();
                return;
            }

            // FIX 1: let the user choose page size instead of hardcoding it
            Console.Write("Enter page size (reviews per page): ");
            if (!int.TryParse(Console.ReadLine(), out int pageSize) || pageSize < 1)
            {
                Console.WriteLine("Invalid page size. Defaulting to 5.");
                pageSize = 5;
            }

            int totalReviews = product.Reviews.Count;
            int totalPages = (int)Math.Ceiling((double)totalReviews / pageSize);
            int page = 1;

            // FIX 2: loop so the user can navigate between pages
            while (true)
            {
                Console.Clear();

                var reviews = product.Reviews
                                     .OrderByDescending(r => r.ReviewDate)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();

                // Header
                Console.WriteLine("================================");
                Console.WriteLine($"Reviews for: {product.Name}");
                Console.WriteLine($"Overall Rating: {product.OverallRating:F1}/5");
                Console.WriteLine($"Total Reviews:  {totalReviews}");
                Console.WriteLine($"Page {page} of {totalPages}");
                Console.WriteLine("================================");

                foreach (var r in reviews)
                {
                    Console.WriteLine($"  Rating:  {r.Rating}/5");
                    Console.WriteLine($"  Comment: {r.Comment}");
                    Console.WriteLine($"  Date:    {r.ReviewDate:dd/MM/yyyy}");
                    Console.WriteLine("  --------------------------------");
                }

                // FIX 3: navigation options based on current page position
                Console.WriteLine();

                if (page > 1)
                    Console.WriteLine("P - Previous page");

                if (page < totalPages)
                    Console.WriteLine("N - Next page");

                Console.WriteLine("Q - Quit");
                Console.Write("Choice: ");

                string input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "N" && page < totalPages)
                {
                    page++;
                }
                else if (input == "P" && page > 1)
                {
                    page--;
                }
                else if (input == "Q")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                }
            }
        }

        public static void GetAllReviews()
        {
            Console.WriteLine("Enter product ID:");
            if (!int.TryParse(Console.ReadLine()?.Trim(), out int id))
            {
                Console.WriteLine("Invalid product ID.");
                Console.ReadKey();
                return;
            }

            Product product = db.Products
                                 .Include(p => p.Reviews)
                                 .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                Console.WriteLine($"Product with ID {id} not found.");
                Console.ReadKey();
                return;
            }

            if (!product.Reviews.Any())
            {
                Console.WriteLine($"No reviews yet for {product.Name}.");
                Console.ReadKey();
                return;
            }

            // FIX 1: let the user choose page size instead of hardcoding it
            Console.Write("Enter page size (reviews per page): ");
            if (!int.TryParse(Console.ReadLine(), out int pageSize) || pageSize < 1)
            {
                Console.WriteLine("Invalid page size. Defaulting to 5.");
                pageSize = 5;
            }

            int totalReviews = product.Reviews.Count;
            int totalPages = (int)Math.Ceiling((double)totalReviews / pageSize);
            int page = 1;

            // FIX 2: loop so the user can navigate between pages
            while (true)
            {
                Console.Clear();

                var reviews = product.Reviews
                                     .OrderByDescending(r => r.ReviewDate)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .ToList();

                // Header
                Console.WriteLine("================================");
                Console.WriteLine($"Reviews for: {product.Name}");
                Console.WriteLine($"Overall Rating: {product.OverallRating:F1}/5");
                Console.WriteLine($"Total Reviews:  {totalReviews}");
                Console.WriteLine($"Page {page} of {totalPages}");
                Console.WriteLine("================================");

                foreach (var r in reviews)
                {
                    Console.WriteLine($"  Rating:  {r.Rating}/5");
                    Console.WriteLine($"  Comment: {r.Comment}");
                    Console.WriteLine($"  Date:    {r.ReviewDate:dd/MM/yyyy}");
                    Console.WriteLine("  --------------------------------");
                }

                // FIX 3: navigation options based on current page position
                Console.WriteLine();

                if (page > 1)
                    Console.WriteLine("P - Previous page");

                if (page < totalPages)
                    Console.WriteLine("N - Next page");

                Console.WriteLine("Q - Quit");
                Console.Write("Choice: ");

                string input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "N" && page < totalPages)
                {
                    page++;
                }
                else if (input == "P" && page > 1)
                {
                    page--;
                }
                else if (input == "Q")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                    Console.ReadKey();
                }
            }
        }
        public static void EditReview()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                Console.ReadKey();
                return;
            }

            // FIX 1: Include Product so we can display the product name instead of just the ID
            var myReviews = db.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == loggedInUser.Id)
                .ToList();

            if (!myReviews.Any())
            {
                Console.WriteLine("You have no reviews yet.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("=== Your Reviews ===");
            foreach (var r in myReviews)
            {
                Console.WriteLine($"  Review ID : {r.Review_Id}");
                Console.WriteLine($"  Product   : {r.Product.Name}");   // FIX 2: name not just ID
                Console.WriteLine($"  Rating    : {r.Rating}/5");
                Console.WriteLine($"  Comment   : {r.Comment}");
                Console.WriteLine($"  Date      : {r.ReviewDate:dd/MM/yyyy}");
                Console.WriteLine("  --------------------------------");
            }

            Console.Write("Enter Review ID to edit/delete: ");
            if (!int.TryParse(Console.ReadLine(), out int revId))
            {
                Console.WriteLine("Invalid input.");
                Console.ReadKey();
                return;
            }

            // Ownership enforced: query filters by both Review_Id AND loggedInUser.Id
            var review = db.Reviews
                .Include(r => r.Product)
                .FirstOrDefault(r => r.Review_Id == revId && r.UserId == loggedInUser.Id);

            if (review == null)
            {
                Console.WriteLine("Review not found or you do not have permission to edit it.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("================================");
            Console.WriteLine($"Editing review for: {review.Product.Name}");
            Console.WriteLine($"Current Rating : {review.Rating}/5");
            Console.WriteLine($"Current Comment: {review.Comment}");
            Console.WriteLine("================================");

            Console.WriteLine("1. Edit Review");
            Console.WriteLine("2. Delete Review");
            Console.Write("Choose option: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid choice.");
                Console.ReadKey();
                return;
            }

            if (choice == 1)
            {
                Console.Write("Enter new comment: ");
                string newComm = Console.ReadLine()?.Trim();

                // FIX 3: reject empty comment
                if (string.IsNullOrWhiteSpace(newComm))
                {
                    Console.WriteLine("Comment cannot be empty.");
                    Console.ReadKey();
                    return;
                }

                Console.Write("Enter new rating (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out int newRate) || newRate < 1 || newRate > 5)
                {
                    Console.WriteLine("Invalid rating. Must be between 1 and 5.");
                    Console.ReadKey();
                    return;
                }

                review.Comment = newComm;
                review.Rating = newRate;
                db.SaveChanges();

                // FIX 4: reload product reviews to show updated overall rating
                var updatedProduct = db.Products
                    .Include(p => p.Reviews)
                    .FirstOrDefault(p => p.Id == review.ProductId);

                Console.WriteLine("================================");
                Console.WriteLine("Review updated successfully!");
                Console.WriteLine($"New Rating    : {review.Rating}/5");
                Console.WriteLine($"New Comment   : {review.Comment}");
                Console.WriteLine($"Product Rating: {updatedProduct.OverallRating:F1}/5");
                Console.WriteLine("================================");
                Console.ReadKey();
            }
            else if (choice == 2)
            {
                Console.Write("Are you sure you want to delete this review? (y/n): ");
                string confirm = Console.ReadLine()?.Trim().ToLower();

                if (confirm == "y" || confirm == "yes")
                {
                    db.Reviews.Remove(review);
                    db.SaveChanges();

                    // FIX 5: reload product reviews to show updated overall rating after delete
                    var updatedProduct = db.Products
                        .Include(p => p.Reviews)
                        .FirstOrDefault(p => p.Id == review.ProductId);

                    Console.WriteLine("================================");
                    Console.WriteLine("Review deleted successfully.");
                    Console.WriteLine($"Product: {review.Product.Name}");
                    Console.WriteLine($"Updated Product Rating: {updatedProduct.OverallRating:F1}/5");
                    Console.WriteLine("================================");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Delete cancelled.");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                Console.ReadKey();
            }
        }
        // ─────────────────────────────────────────────
        //  LOGOUT
        // ─────────────────────────────────────────────

        public static bool Logout()
        {
            Console.Write("Are you sure you want to logout? (yes/no): ");
            string confirmLogout = Console.ReadLine() ?? string.Empty;

            if (confirmLogout.Trim().ToLower() == "yes")
            {
                loggedInUser = null;
                Console.WriteLine("Logging out...");
                Console.WriteLine("Thank you for using E-Commerce System!");
                return true;
            }

            Console.WriteLine("Exit cancelled. Returning to main menu...");
            return false;
        }

        // ─────────────────────────────────────────────
        //  MENUS
        // ─────────────────────────────────────────────

        public static void Menu()
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine("\nWelcome to the E-Commerce System:");
                Console.WriteLine("----------------------------------");
                Console.WriteLine("1. User API");
                Console.WriteLine("2. Product API");
                Console.WriteLine("3. Order API");
                Console.WriteLine("4. Review API");
                Console.WriteLine("5. Logout");

                Console.Write("Enter your choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid number!");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 1: UserMenu(); break;
                    case 2: ProductMenu(); break;
                    case 3: OrderMenu(); break;
                    case 4: ReviewMenu(); break;
                    case 5: logout = Logout(); break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (!logout)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void UserMenu()
        {
            Console.WriteLine("\nUser API:");
            Console.WriteLine("1. Get user details by ID");

            if (!int.TryParse(Console.ReadLine(), out int choice)) return;

            switch (choice)
            {
                case 1: GetUserDetails(); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        static void ProductMenu()
        {
            Console.WriteLine("\nProduct API:");
            Console.WriteLine("1. Add new product (Admin only)");
            Console.WriteLine("2. Update product (Admin only)");
            Console.WriteLine("3. Get products (pagination/filter)");
            Console.WriteLine("4. Get product by ID");

            if (!int.TryParse(Console.ReadLine(), out int choice)) return;

            switch (choice)
            {
                case 1: AddProduct(); break;
                case 2: UpdateProduct(); break;
                case 3: FilterProducts(); break;
                case 4: GetProductDetails(); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        static void OrderMenu()
        {
            Console.WriteLine("\nOrder API:");
            Console.WriteLine("1. Place new order");
            Console.WriteLine("2. Get all orders for user");
            Console.WriteLine("3. Get order by ID");

            if (!int.TryParse(Console.ReadLine(), out int choice)) return;

            switch (choice)
            {
                case 1: PlaceNewOrder(); break;
                case 2: GetUserOrders(); break;
                case 3: OrderDetail(); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        static void ReviewMenu()
        {
            Console.WriteLine("\nReview API:");
            Console.WriteLine("1. Add review");
            Console.WriteLine("2. Get reviews for product");
            Console.WriteLine("3. Edit / Delete review");

            if (!int.TryParse(Console.ReadLine(), out int choice)) return;

            switch (choice)
            {
                case 1: AddReview(); break;
                case 2: GetAllReviews(); break;
                case 3: EditReview(); break;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        // ─────────────────────────────────────────────
        //  ENTRY POINT
        // ─────────────────────────────────────────────

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== E-Commerce System =====");
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
                        // FIX 14: ReadKey already inside RegisterUser(); no double pause needed
                        break;

                    case 2:
                        LoginUser();
                        if (loggedInUser != null)
                            Menu();
                        else
                        {
                            // FIX 15: pause so "Invalid email or password" is visible
                            Console.ReadKey();
                        }
                        break;

                    case 3:
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}