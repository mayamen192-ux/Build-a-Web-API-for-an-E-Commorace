using Build_a_Web_API_for_an_E_Commorace.Models;
using System;

namespace Build_a_Web_API_for_an_E_Commorace
{
    internal class Program
    {
        static ApplicationDbContext db = new ApplicationDbContext();
        static User loggedInUser = null;

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
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
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
                        if (loggedInUser == null)
                        {
                            Console.WriteLine("No user is logged in!");
                            return;
                        }

                        loggedInUser = null;
                        Console.WriteLine("Logout successful!");
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


                    Console.WriteLine("Enter User Name");
                    string name = Console.ReadLine();

                    Console.WriteLine("Enter User Email");
                    string email = Console.ReadLine();

                    Console.WriteLine("Enter User Phone Number");
                    string phone = Console.ReadLine();

                    Console.WriteLine("Enter User Role");
                    string role = Console.ReadLine();
                    // Basic validation
                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(email) ||
                        string.IsNullOrWhiteSpace(phone) ||
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
                        phone = phone,
                        role = role,
                        createdAt = date
                    });

                    db.SaveChanges();

                    Console.WriteLine("User Registered Successfully!");
                    break;


                case 2:
                    Console.WriteLine("Enter Email:");
                    string emailLogin = Console.ReadLine();
                    Console.WriteLine("Enter Password:");
                    string PasswordLogin = Console.ReadLine();

                    // find user in database
                    var user = db.Users.FirstOrDefault(u => u.email == emailLogin && u.password == PasswordLogin);
                    if (user == null)
                    {
                        Console.WriteLine("Invalid email or password!");
                        return;
                    }
                    Console.WriteLine("Login Successful!");
                    break;

                case 3:
                    Console.WriteLine("Enter User ID:");
                    int id = int.Parse(Console.ReadLine());
                    User user1 = db.Users.Find(id);
                    if (user1 == null)
                    {
                        Console.WriteLine("User not found!");
                        return;
                    }
                    Console.WriteLine("User Name:" + user1.name + "  ,User Email:" + user1.email + "  ,User Phone:" + user1.phone + "  , User Role:" + user1.role + "  , Date:" + user1.createdAt);

                    Console.WriteLine("Get user details...");
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
                    Console.WriteLine("Enter Admin Role:");
                    string role = Console.ReadLine();

                    if (role.ToLower() != "admin")
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
                    break;

                case 2:
                   
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
                    break;

                case 3:
                    //Console.WriteLine("....List products...");

                    //List<Product> proList = db.Products.ToList();
                    //for(int i=0; i< proList.Count; i++)
                    //{
                    //    Console.WriteLine("Product Name:" + proList[i].Name + ", Description:" + proList[i].Description +
                    //        " , Price" + proList[i].Price + " , Stock:" + proList[i].Stock + " , Overall Rating:" + proList[i].OverallRating);

                    //}
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

                    // FILTERING
                    var query = db.Products.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(nameFilter))
                    {
                        query = query.Where(p => p.Name.Contains(nameFilter));
                    }

                    query = query.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

                    // PAGINATION
                    var products = query
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    // DISPLAY
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

                    break;

                case 4:

                    Console.WriteLine("Enter Product Id:");
                    int id2 = int.Parse(Console.ReadLine());
                    Product Pp = db.Products.Find(id2);
                    Console.WriteLine("Product details...");
                    Console.WriteLine("Product Name:" + Pp.Name + ", Description:" + Pp.Description +
                   " , Price" + Pp.Price + " , Stock:" + Pp.Stock + " , Overall Rating:" + Pp.OverallRating);

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

                    db.Products.Update(product);

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
                    break;

                case 2:
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
                    break;

                case 3:
                    // Check authentication
                    if (loggedInUser == null)
                    {
                        Console.WriteLine("Access denied! Please login first.");
                        return;
                    }

                    Console.WriteLine("Enter Order Id:");
                    int order_ids = int.Parse(Console.ReadLine());

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

                    Console.WriteLine(
                        "User Id: " + ord.UserId +
                        " , Order Id: " + ord.order_Id +
                        " , Order Date: " + ord.orderDate +
                        " , Total Amount: " + ord.TotalAmount
                    );


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
                    // Check authentication
                    if (loggedInUser == null)
                    {
                        Console.WriteLine("Access denied! Please login first.");
                        return;
                    }

                    Console.WriteLine("Enter Product Id:");
                    int productId = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter Rating:");
                    int rating = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter Comment:");
                    string comment = Console.ReadLine();

                    // Check if product exists
                    Product product = db.Products.Find(productId);

                    if (product == null)
                    {
                        Console.WriteLine("Product not found!");
                        return;
                    }

                    // Check if user previously ordered this product
                    bool hasOrdered = db.orderProducts
                        .Any(op =>
                            op.ProductId == productId &&
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
                    break;

                case 2:
                    Console.WriteLine("Enter Product Id:");
                    int productId2 = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter Page Number:");
                    int pageNumber = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter Page Size:");
                    int pageSize = int.Parse(Console.ReadLine());

                    // Get reviews for specific product with pagination
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
                    break;

                case 3:
                    Console.WriteLine("Enter option 1 or 2");
                    Console.WriteLine("1.Update Review ");
                    Console.WriteLine("2.Delete Review");
                    int option=int.Parse(Console.ReadLine());
                    if (option == 1)
                    {


                        // Check authentication
                        if (loggedInUser == null)
                        {
                            Console.WriteLine("Access denied! Please login first.");
                            return;
                        }

                        Console.WriteLine("Enter Review Id:");
                        int reviewId = int.Parse(Console.ReadLine());

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
                        int newRating = int.Parse(Console.ReadLine());

                        Console.WriteLine("Enter New Comment:");
                        string newComment = Console.ReadLine();

                        // Update review
                        review2.Rating = newRating;
                        review2.Comment = newComment;
                        review2.ReviewDate = DateTime.Now;

                        db.Reviews.Update(review2);
                        db.SaveChanges();

                        Console.WriteLine("Review updated successfully!");
                        Console.WriteLine("Update review...");
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
                        int reviewId2 = int.Parse(Console.ReadLine());

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
                            break;

                    
            
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        
        
            }
        }
    }
