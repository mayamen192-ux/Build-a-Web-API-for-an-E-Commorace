using Build_a_Web_API_for_an_E_Commorace.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Build_a_Web_API_for_an_E_Commorace
{
    internal class Program
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        static User loggedInUser = null;
        // Defined Function
        static  public void RegisterUser()
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

            // Basic validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role))
            {
                Console.WriteLine("All fields are required!");
                return;
            }

            DateTime date = DateTime.Now;

            db.Users.Add(new User
            {
                name = name,
                email = email,
                password = password,
                phone = phone,
                role = role,
                createdAt = date
            });

            db.SaveChanges();

            Console.WriteLine("User Registered Successfully!");
        }
        static public  void LoginUser()
        {
            Console.WriteLine("Enter Email:");
            string emailLogin = Console.ReadLine();

            Console.WriteLine("Enter Password:");
            string PasswordLogin = Console.ReadLine();

            // find user in database
            var user = db.Users.FirstOrDefault
            (
                u => u.email == emailLogin && u.password == PasswordLogin
            );

            if (user == null)
            {
                Console.WriteLine("Invalid email or password!");
                return;
            }

            Console.WriteLine("Login Successful!");
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
        static  public void AddProduct()
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
            decimal ProPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter Product Stock:");
            int ProStock = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Product overall Rating:");
            decimal overallRating = decimal.Parse(Console.ReadLine());

            db.Products.Add(new Product
            {
                Name = ProName,
                Description = ProDescription,
                Price = ProPrice,
                Stock = ProStock
            });

            db.SaveChanges();

            Console.WriteLine("Added product Successfully");
        }
        static  public void UpdateProduct()
        {
            // Check authentication
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                return;
            }

            // Check admin role
            if (loggedInUser.role.ToLower() != "admin")
            {
                Console.WriteLine("Access denied! Admin only.");
                return;
            }

            Console.WriteLine("Enter Product Id:");
            int proID = int.Parse(Console.ReadLine());

            // Find product
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
            decimal price = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter Product Stock:");
            int pStock = int.Parse(Console.ReadLine());

            // Update product
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
            Console.WriteLine("Enter Product Id:");
            int id2 = int.Parse(Console.ReadLine());

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
        static public void PlaceOrder()
        {
            Console.WriteLine("Enter Order name:");
            string orderName = Console.ReadLine();

            Console.WriteLine("Enter User Id:");
            int userId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Product Id:");
            int productId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Quantity:");
            int quantity = int.Parse(Console.ReadLine());

            // Get product from database
            var product = db.Products.Find(productId);

            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            // Check if price is null
            if (product.Price == null)
            {
                Console.WriteLine("Product price is not available!");
                return;
            }

            // Check stock
            if (product.Stock < quantity)
            {
                Console.WriteLine("Not enough stock available!");
                return;
            }

            // Calculate total amount
            decimal totalAmount = product.Price.Value * quantity;

            // Reduce stock
            product.Stock -= quantity;

            // Create order
            Order order = new Order
            {
                UserId = userId,
                orderDate = DateTime.Now,

                OrderProducts = new List<OrderProducts>
        {
            new OrderProducts
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price.Value
            }
        }
            };

            db.Orders.Add(order);

            db.SaveChanges();

            Console.WriteLine("Order placed successfully!");
            Console.WriteLine("Total Amount: " + totalAmount);
        }
        static public void GetUserOrders()
        {
            // Check authentication
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                return;
            }

            Console.WriteLine("User orders...");

            // Get ONLY logged-in user's orders
            List<Order> orders = db.Orders
                .Where(o => o.UserId == loggedInUser.Id)
                .ToList();

            if (orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            // Display orders
            for (int i = 0; i < orders.Count; i++)
            {
                Console.WriteLine(
                    "User Id: " + orders[i].UserId +
                    " , Order Id: " + orders[i].order_Id +
                    " , Order Date: " + orders[i].orderDate
                );
            }
        }

        static public void GetOrderById()
        {
            // Check authentication
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                return;
            }

            Console.WriteLine("Enter Order Id:");

            int order_ids;

            if (!int.TryParse(Console.ReadLine(), out order_ids))
            {
                Console.WriteLine("Invalid Order Id!");
                return;
            }

            // Get ONLY logged-in user's order
            Order ord = db.Orders
                .FirstOrDefault(o =>
                    o.order_Id == order_ids &&
                    o.UserId == loggedInUser.Id);

            if (ord == null)
            {
                Console.WriteLine("Order not found or access denied!");
                return;
            }

            Console.WriteLine("Order details...");

            decimal total = ord.TotalAmount;

            Console.WriteLine(
                "User Id: " + ord.UserId +
                " , Order Id: " + ord.order_Id +
                " , Order Date: " + ord.orderDate +
                " , Total Amount: " + total
            );
        }
        static public void AddProductReview()
        {
            // Check authentication
            if (loggedInUser == null)
            {
                Console.WriteLine("Access denied! Please login first.");
                return;
            }

            Console.WriteLine("Enter Product Id:");

            int productId;

            if (!int.TryParse(Console.ReadLine(), out productId))
            {
                Console.WriteLine("Invalid Product Id!");
                return;
            }

            Console.WriteLine("Enter Rating:");

            int rating;

            if (!int.TryParse(Console.ReadLine(), out rating))
            {
                Console.WriteLine("Invalid Rating!");
                return;
            }

            // Validate rating
            if (rating < 1 || rating > 5)
            {
                Console.WriteLine("Rating must be between 1 and 5.");
                return;
            }

            Console.WriteLine("Enter Comment:");
            string comment = Console.ReadLine();

            // Check if product exists
            Product product = db.Products.Find(productId);

            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            // Check if user purchased product
            bool hasOrdered = db.orderProducts
                .Any(op =>
                    op.ProductId == productId &&
                    op.Order != null &&
                    op.Order.UserId == loggedInUser.Id);

            if (!hasOrdered)
            {
                Console.WriteLine("You can only review products you purchased!");
                return;
            }

            // Create review
            Review review = new Review
            {
                UserId = loggedInUser.Id,
                ProductId = productId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            db.Reviews.Add(review);

            db.SaveChanges();

            Console.WriteLine("Review Added Successfully!");
        }
        static public void GetProductReviews()
        {
            Console.WriteLine("Enter Product Id:");

            int productId2;

            if (!int.TryParse(Console.ReadLine(), out productId2))
            {
                Console.WriteLine("Invalid Product Id!");
                return;
            }

            Console.WriteLine("Enter Page Number:");

            int pageNumber;

            if (!int.TryParse(Console.ReadLine(), out pageNumber))
            {
                Console.WriteLine("Invalid Page Number!");
                return;
            }

            Console.WriteLine("Enter Page Size:");

            int pageSize;

            if (!int.TryParse(Console.ReadLine(), out pageSize))
            {
                Console.WriteLine("Invalid Page Size!");
                return;
            }

            // Validate pagination values
            if (pageNumber <= 0 || pageSize <= 0)
            {
                Console.WriteLine("Page Number and Page Size must be greater than 0.");
                return;
            }

            // Check if product exists
            Product product = db.Products.Find(productId2);

            if (product == null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            // Get reviews with pagination
            List<Review> reviews = db.Reviews
                .Where(r => r.ProductId == productId2)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (reviews.Count == 0)
            {
                Console.WriteLine("No reviews found.");
                return;
            }

            // Display reviews
            for (int i = 0; i < reviews.Count; i++)
            {
                Console.WriteLine(
                    "User Id: " + reviews[i].UserId +
                    " , Product Id: " + reviews[i].ProductId +
                    " , Comment: " + reviews[i].Comment +
                    " , Rating: " + reviews[i].Rating +
                    " , Review Date: " + reviews[i].ReviewDate
                );
            }

            Console.WriteLine("Reviews displayed successfully!");
        }
        static public void ManageReview()
        {
            Console.WriteLine("Enter option 1 or 2");
            Console.WriteLine("1.Update Review ");
            Console.WriteLine("2.Delete Review");

            int option;

            if (!int.TryParse(Console.ReadLine(), out option))
            {
                Console.WriteLine("Invalid option!");
                return;
            }

            if (option == 1)
            {
                // Check authentication
                if (loggedInUser == null)
                {
                    Console.WriteLine("Access denied! Please login first.");
                    return;
                }

                Console.WriteLine("Enter Review Id:");

                int reviewId;

                if (!int.TryParse(Console.ReadLine(), out reviewId))
                {
                    Console.WriteLine("Invalid Review Id!");
                    return;
                }

                // Find review created by logged-in user
                Review review2 = db.Reviews
                    .FirstOrDefault(r =>
                        r.Review_Id == reviewId &&
                        r.UserId == loggedInUser.Id);

                if (review2 == null)
                {
                    Console.WriteLine("Review not found or access denied!");
                    return;
                }

                Console.WriteLine("Enter New Rating:");

                int newRating;

                if (!int.TryParse(Console.ReadLine(), out newRating))
                {
                    Console.WriteLine("Invalid Rating!");
                    return;
                }

                // Validate rating
                if (newRating < 1 || newRating > 5)
                {
                    Console.WriteLine("Rating must be between 1 and 5.");
                    return;
                }

                Console.WriteLine("Enter New Comment:");
                string newComment = Console.ReadLine();

                // Update review
                review2.Rating = newRating;
                review2.Comment = newComment;
                review2.ReviewDate = DateTime.Now;

                db.Reviews.Update(review2);

                db.SaveChanges();

                Console.WriteLine("Review updated successfully!");
            }
            else if (option == 2)
            {
                // Check authentication
                if (loggedInUser == null)
                {
                    Console.WriteLine("Access denied! Please login first.");
                    return;
                }

                Console.WriteLine("Enter Review Id:");

                int reviewId2;

                if (!int.TryParse(Console.ReadLine(), out reviewId2))
                {
                    Console.WriteLine("Invalid Review Id!");
                    return;
                }

                // Find review created by logged-in user
                Review review3 = db.Reviews
                    .FirstOrDefault(r =>
                        r.Review_Id == reviewId2 &&
                        r.UserId == loggedInUser.Id);

                if (review3 == null)
                {
                    Console.WriteLine("Review not found or access denied!");
                    return;
                }

                // Delete review
                db.Reviews.Remove(review3);

                db.SaveChanges();

                Console.WriteLine("Review deleted successfully!");
            }
            else
            {
                Console.WriteLine("Invalid option!");
            }
        }
        static  public void LoginNewUser()
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
        static  public void LogoutUser()
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("No user is logged in!");
                return;
            }

            loggedInUser = null;

            Console.WriteLine("Logout successful!");
        }
        static void Main(string[] args)
        {
            db.Database.EnsureCreated();


            while (true)
            {
                Console.WriteLine("\nWelcome to the E-Commerce System:");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1. User Log-in");
                Console.WriteLine("2. User API");
                Console.WriteLine("3.Product API");
                Console.WriteLine("4. Order API");
                Console.WriteLine("5. Review API");
                Console.WriteLine("6.  User Log _out");

                Console.Write("Enter your choice: ");
                
                int choice;
      if (!int.TryParse(Console.ReadLine(), out choice))
{
    Console.WriteLine("Invalid number!");
    return;
}

                switch (choice)
                {
                    case 1:
                        LoginNewUser();
                        break;
                    case 2:
                        UserMenu();
                        break;

                    case 3:
                        ProductMenu();
                        break;

                    case 4:
                        OrderMenu();
                        break;

                    case 5:
                        ReviewMenu();
                        break;

                    case 6:
                        LogoutUser();
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
            Console.WriteLine("1. Register new user");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Get user details by ID");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:

                    RegisterUser();

                    break;


                case 2:
                    LoginUser();
                    break;

                case 3:
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
                    PlaceOrder();
                    break;

                case 2:
                    GetUserOrders();
                    break;

                case 3:
                    GetOrderById();


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
                    AddProductReview();
                    break;

                case 2:
                    GetProductReviews();
                    break;

                case 3:
                    ManageReview();
                            break;

                    
            
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        
        
            }
        }
    }
