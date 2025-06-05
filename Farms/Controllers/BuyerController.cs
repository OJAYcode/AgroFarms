using Microsoft.AspNetCore.Mvc;
using Farms.Services;
using Farms.Models;
using Farms.Models.ViewModels;
using Farms.Data;
using MongoDB.Bson;
using Farms.Utilities;

namespace Farms.Controllers
{
    public class BuyerController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly MongoDbContext _context;
        private readonly IStaticProductService _staticProductService;
        private readonly IEnhancedCartService _enhancedCartService;

        public BuyerController(IUserService userService, IProductService productService, 
            ICartService cartService, IOrderService orderService, MongoDbContext context,
            IStaticProductService staticProductService, IEnhancedCartService enhancedCartService)
        {
            _userService = userService;
            _productService = productService;
            _cartService = cartService;
            _orderService = orderService;
            _context = context;
            _staticProductService = staticProductService;
            _enhancedCartService = enhancedCartService;
        }        public async Task<IActionResult> Dashboard()
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            var buyerId = HttpContext.Session.GetString("UserId")!;
            var orders = await _orderService.GetOrdersByBuyerAsync(buyerId);
            var cartCount = await _enhancedCartService.GetCartItemCountAsync(buyerId);

            ViewBag.TotalOrders = orders.Count;
            ViewBag.CartItemCount = cartCount;
            ViewBag.TotalSpent = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalAmount);
            ViewBag.RecentOrders = orders.Take(5).ToList();

            return View();
        }        public IActionResult Products(string category = "", string search = "", int page = 1)
        {
            var pageSize = 12;
            
            // Use StaticProductService for consistent product management
            var allProducts = _staticProductService.GetAllProducts();
            var filteredProducts = _staticProductService.FilterProducts(
                products: allProducts,
                category: category,
                searchTerm: search,
                minPrice: null,
                maxPrice: null,
                isOrganic: null,
                sortBy: "name"
            );
            
            var categories = allProducts.Select(p => p.Category).Distinct().ToList();
            
            var totalProducts = filteredProducts.Count();
            var pagedProducts = filteredProducts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new ProductListViewModel
            {
                Products = pagedProducts,
                Categories = categories,
                SelectedCategory = category,
                SearchTerm = search,
                PageNumber = page,
                PageSize = pageSize,
                TotalProducts = totalProducts
            };

            return View(viewModel);
        }

        private List<Product> GetStaticProducts()
        {
            return new List<Product>
            {
                // Vegetables
                new Product
                {
                    Id = "1",
                    Name = "Organic Tomatoes",
                    Description = "Fresh, juicy organic tomatoes grown without pesticides. Perfect for salads, cooking, and sandwiches.",
                    Price = 4.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer1",
                    FarmerName = "Green Valley Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1546094096-0df4bcaaa337?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    IsOrganic = true,
                    StockQuantity = 25
                },
                new Product
                {
                    Id = "2",
                    Name = "Fresh Carrots",
                    Description = "Crisp, sweet carrots harvested at peak freshness. Rich in vitamins and perfect for snacking or cooking.",
                    Price = 3.49m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer2",
                    FarmerName = "Sunset Organic Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1445282768818-728615cc910a?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    IsOrganic = true,
                    StockQuantity = 30
                },
                new Product
                {
                    Id = "3",
                    Name = "Bell Peppers Mix",
                    Description = "Colorful mix of red, yellow, and green bell peppers. Sweet, crunchy, and packed with vitamins.",
                    Price = 5.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer1",
                    FarmerName = "Green Valley Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1563565375-f3fdfdbefa83?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    IsOrganic = false,
                    StockQuantity = 20
                },
                new Product
                {
                    Id = "4",
                    Name = "Fresh Broccoli",
                    Description = "Nutrient-dense broccoli crowns, freshly harvested. Great for steaming, stir-frying, or eating raw.",
                    Price = 4.29m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer3",
                    FarmerName = "Meadow Fresh Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1459411621453-7b03977f4bfc?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsOrganic = true,
                    StockQuantity = 18
                },
                new Product
                {
                    Id = "5",
                    Name = "Baby Spinach",
                    Description = "Tender baby spinach leaves, perfect for salads and smoothies. Grown without chemicals.",
                    Price = 6.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer2",
                    FarmerName = "Sunset Organic Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsOrganic = true,
                    StockQuantity = 15
                },
                new Product
                {
                    Id = "6",
                    Name = "Sweet Corn",
                    Description = "Sweet, tender corn on the cob. Harvested at peak sweetness for maximum flavor.",
                    Price = 1.99m,
                    Category = "Vegetables",
                    Unit = "each",
                    FarmerId = "farmer4",
                    FarmerName = "Country Acres",
                    ImageUrl = "https://images.unsplash.com/photo-1551754655-cd27e38d2076?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    IsOrganic = false,
                    StockQuantity = 40
                },

                // Fruits
                new Product
                {
                    Id = "7",
                    Name = "Honeycrisp Apples",
                    Description = "Crisp, sweet Honeycrisp apples with the perfect balance of sweetness and tartness.",
                    Price = 5.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer5",
                    FarmerName = "Orchard Hill Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    IsOrganic = true,
                    StockQuantity = 35
                },
                new Product
                {
                    Id = "8",
                    Name = "Fresh Strawberries",
                    Description = "Sweet, juicy strawberries picked at peak ripeness. Perfect for desserts or eating fresh.",
                    Price = 8.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer6",
                    FarmerName = "Berry Patch Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1464965911861-746a04b4bca6?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 22
                },
                new Product
                {
                    Id = "9",
                    Name = "Organic Bananas",
                    Description = "Ripe, sweet organic bananas. Great for snacking, smoothies, or baking.",
                    Price = 2.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer7",
                    FarmerName = "Tropical Grove",
                    ImageUrl = "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    IsOrganic = true,
                    StockQuantity = 50
                },
                new Product
                {
                    Id = "10",
                    Name = "Fresh Blueberries",
                    Description = "Plump, sweet blueberries bursting with antioxidants and flavor. Perfect for breakfast or snacking.",
                    Price = 12.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer6",
                    FarmerName = "Berry Patch Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1498557850523-fd3d118b962e?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsOrganic = true,
                    StockQuantity = 12
                },
                new Product
                {
                    Id = "11",
                    Name = "Valencia Oranges",
                    Description = "Sweet, juicy Valencia oranges perfect for fresh juice or eating. High in vitamin C.",
                    Price = 4.49m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer7",
                    FarmerName = "Tropical Grove",
                    ImageUrl = "https://images.unsplash.com/photo-1547036967-23d11aacaee0?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                    IsOrganic = false,
                    StockQuantity = 28
                },

                // Grains
                new Product
                {
                    Id = "12",
                    Name = "Organic Brown Rice",
                    Description = "Wholesome organic brown rice with nutty flavor and perfect texture. Great source of fiber.",
                    Price = 6.99m,
                    Category = "Grains",
                    Unit = "lb",
                    FarmerId = "farmer8",
                    FarmerName = "Golden Grain Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1586201375761-83865001e31c?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    IsOrganic = true,
                    StockQuantity = 45
                },
                new Product
                {
                    Id = "13",
                    Name = "Quinoa",
                    Description = "Premium quinoa, a complete protein grain perfect for healthy meals and salads.",
                    Price = 9.99m,
                    Category = "Grains",
                    Unit = "lb",
                    FarmerId = "farmer8",
                    FarmerName = "Golden Grain Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1586024229781-c6b4d6d9b2e6?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    IsOrganic = true,
                    StockQuantity = 25
                },
                new Product
                {
                    Id = "14",
                    Name = "Whole Wheat Flour",
                    Description = "Stone-ground whole wheat flour perfect for baking bread, muffins, and pastries.",
                    Price = 4.99m,
                    Category = "Grains",
                    Unit = "lb",
                    FarmerId = "farmer9",
                    FarmerName = "Heritage Mills",
                    ImageUrl = "https://images.unsplash.com/photo-1574323347407-f5e1ad6d020b?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    IsOrganic = true,
                    StockQuantity = 30
                },

                // Herbs
                new Product
                {
                    Id = "15",
                    Name = "Fresh Basil",
                    Description = "Aromatic fresh basil leaves, perfect for Italian dishes, pesto, and garnishing.",
                    Price = 3.99m,
                    Category = "Herbs",
                    Unit = "bunch",
                    FarmerId = "farmer10",
                    FarmerName = "Herb Garden Co",
                    ImageUrl = "https://images.unsplash.com/photo-1618375569909-3c8616cf6b96?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 20
                },
                new Product
                {
                    Id = "16",
                    Name = "Rosemary",
                    Description = "Fresh rosemary sprigs with intense aroma, perfect for roasting and seasoning meats.",
                    Price = 2.99m,
                    Category = "Herbs",
                    Unit = "bunch",
                    FarmerId = "farmer10",
                    FarmerName = "Herb Garden Co",
                    ImageUrl = "https://images.unsplash.com/photo-1583475506388-18bdde21cd02?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsOrganic = true,
                    StockQuantity = 18
                },
                new Product
                {
                    Id = "17",
                    Name = "Fresh Cilantro",
                    Description = "Vibrant cilantro with fresh, citrusy flavor. Essential for Mexican and Asian cuisines.",
                    Price = 2.49m,
                    Category = "Herbs",
                    Unit = "bunch",
                    FarmerId = "farmer10",
                    FarmerName = "Herb Garden Co",
                    ImageUrl = "https://images.unsplash.com/photo-1615485925450-b2c94b816dc8?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsOrganic = true,
                    StockQuantity = 25
                },

                // Dairy
                new Product
                {
                    Id = "18",
                    Name = "Farm Fresh Milk",
                    Description = "Pure, creamy milk from grass-fed cows. Rich in nutrients and perfect for drinking or cooking.",
                    Price = 4.99m,
                    Category = "Dairy",
                    Unit = "gallon",
                    FarmerId = "farmer11",
                    FarmerName = "Moo Valley Dairy",
                    ImageUrl = "https://images.unsplash.com/photo-1563636619-e9143da7973b?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsOrganic = true,
                    StockQuantity = 15
                },
                new Product
                {
                    Id = "19",
                    Name = "Artisan Cheese",
                    Description = "Handcrafted artisan cheese made from fresh farm milk. Rich, creamy texture with complex flavors.",
                    Price = 12.99m,
                    Category = "Dairy",
                    Unit = "lb",
                    FarmerId = "farmer11",
                    FarmerName = "Moo Valley Dairy",
                    ImageUrl = "https://images.unsplash.com/photo-1486297678162-eb2a19b0a32d?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsOrganic = true,
                    StockQuantity = 8
                },
                new Product
                {
                    Id = "20",
                    Name = "Free-Range Eggs",
                    Description = "Fresh eggs from free-range chickens. Rich, golden yolks and excellent for any meal.",
                    Price = 6.99m,
                    Category = "Dairy",
                    Unit = "dozen",
                    FarmerId = "farmer12",
                    FarmerName = "Happy Hen Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1569288052389-dac9b01ac769?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 24
                },

                // Meat
                new Product
                {
                    Id = "21",
                    Name = "Grass-Fed Beef",
                    Description = "Premium grass-fed beef, tender and flavorful. Raised without hormones or antibiotics.",
                    Price = 18.99m,
                    Category = "Meat",
                    Unit = "lb",
                    FarmerId = "farmer13",
                    FarmerName = "Prairie Meat Co",
                    ImageUrl = "https://images.unsplash.com/photo-1588347818133-93f8e7f7b60c?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsOrganic = true,
                    StockQuantity = 12
                },
                new Product
                {
                    Id = "22",
                    Name = "Free-Range Chicken",
                    Description = "Tender, juicy chicken from free-range birds. Perfect for roasting, grilling, or any recipe.",
                    Price = 8.99m,
                    Category = "Meat",
                    Unit = "lb",
                    FarmerId = "farmer12",
                    FarmerName = "Happy Hen Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1604503468506-a8da13d82791?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    IsOrganic = true,
                    StockQuantity = 10
                },

                // Additional diverse products
                new Product
                {
                    Id = "23",
                    Name = "Cherry Tomatoes",
                    Description = "Sweet, bite-sized cherry tomatoes perfect for salads and snacking. Bursting with flavor.",
                    Price = 5.49m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer1",
                    FarmerName = "Green Valley Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1592841200221-21e1c7d4509b?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    IsOrganic = true,
                    StockQuantity = 20
                },
                new Product
                {
                    Id = "24",
                    Name = "Mixed Greens",
                    Description = "Fresh mix of lettuce, arugula, and spinach. Perfect base for healthy salads.",
                    Price = 7.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer2",
                    FarmerName = "Sunset Organic Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 16
                },
                new Product
                {
                    Id = "25",
                    Name = "Granny Smith Apples",
                    Description = "Tart, crisp Granny Smith apples perfect for baking pies and making applesauce.",
                    Price = 4.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer5",
                    FarmerName = "Orchard Hill Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1568702846914-96b305d2aaeb?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    IsOrganic = false,
                    StockQuantity = 30
                },
                new Product
                {
                    Id = "26",
                    Name = "Organic Lemons",
                    Description = "Bright, juicy organic lemons perfect for cooking, baking, and making fresh lemonade.",
                    Price = 6.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer7",
                    FarmerName = "Tropical Grove",
                    ImageUrl = "https://images.unsplash.com/photo-1590502593747-42a996133562?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    IsOrganic = true,
                    StockQuantity = 18
                },
                new Product
                {
                    Id = "27",
                    Name = "Organic Oats",
                    Description = "Wholesome organic rolled oats, perfect for breakfast porridge and baking.",
                    Price = 5.99m,
                    Category = "Grains",
                    Unit = "lb",
                    FarmerId = "farmer8",
                    FarmerName = "Golden Grain Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1574323347407-f5e1ad6d020b?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    IsOrganic = true,
                    StockQuantity = 35
                },
                new Product
                {
                    Id = "28",
                    Name = "Fresh Thyme",
                    Description = "Fragrant fresh thyme with earthy flavor. Perfect for Mediterranean dishes and seasoning.",
                    Price = 3.49m,
                    Category = "Herbs",
                    Unit = "bunch",
                    FarmerId = "farmer10",
                    FarmerName = "Herb Garden Co",
                    ImageUrl = "https://images.unsplash.com/photo-1582973400074-bb79374c0b1b?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    IsOrganic = true,
                    StockQuantity = 15
                },
                new Product
                {
                    Id = "29",
                    Name = "Greek Yogurt",
                    Description = "Thick, creamy Greek yogurt made from fresh farm milk. High in protein and probiotics.",
                    Price = 7.99m,
                    Category = "Dairy",
                    Unit = "lb",
                    FarmerId = "farmer11",
                    FarmerName = "Moo Valley Dairy",
                    ImageUrl = "https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsOrganic = true,
                    StockQuantity = 12
                },
                new Product
                {
                    Id = "30",
                    Name = "Farm Fresh Butter",
                    Description = "Rich, creamy butter made from fresh cream. Perfect for cooking and baking.",
                    Price = 8.99m,
                    Category = "Dairy",
                    Unit = "lb",
                    FarmerId = "farmer11",
                    FarmerName = "Moo Valley Dairy",
                    ImageUrl = "https://images.unsplash.com/photo-1589985270826-4b7bb135bc9d?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 14
                }
            };
        }        [HttpGet]
        public IActionResult ProductDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Product ID is required");
            }

            // Get product from StaticProductService to ensure consistency
            var product = _staticProductService.GetProductById(id);
            
            if (product == null)
            {
                return NotFound("Product not found");
            }

            // Check if product is available (has stock)
            if (product.StockQuantity <= 0)
            {
                return NotFound("Product not available");
            }
            
            // Set related products for display using StaticProductService
            var allProducts = _staticProductService.GetAllProducts();
            var relatedProducts = allProducts
                .Where(p => p.Category == product.Category && p.Id != product.Id)
                .Take(4)
                .ToList();
            
            ViewBag.RelatedProducts = relatedProducts;
            
            return View(product);
        }        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartViewModel model)
        {
            try
            {
                Console.WriteLine($"AddToCart called with ProductId: '{model?.ProductId}', Quantity: {model?.Quantity}");
                
                // Validate authentication
                if (!IsValidBuyer())
                {
                    Console.WriteLine("AddToCart - User not authenticated");
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Validate model
                if (model == null)
                {
                    Console.WriteLine("AddToCart - Model is null");
                    return Json(new { success = false, message = "Invalid request data" });
                }

                if (string.IsNullOrEmpty(model.ProductId))
                {
                    Console.WriteLine("AddToCart - ProductId is null or empty");
                    return Json(new { success = false, message = "Product ID is required" });
                }

                if (model.Quantity <= 0)
                {
                    Console.WriteLine($"AddToCart - Invalid quantity: {model.Quantity}");
                    return Json(new { success = false, message = "Quantity must be greater than 0" });
                }

                // Get product from StaticProductService to ensure consistency with EnhancedCartService
                var product = _staticProductService.GetProductById(model.ProductId);
                
                if (product == null)
                {
                    Console.WriteLine($"AddToCart - Product not found for ID: '{model.ProductId}'");
                    return Json(new { success = false, message = $"Product not found with ID: {model.ProductId}" });
                }

                // Check stock using StaticProductService
                if (!_staticProductService.IsProductAvailable(model.ProductId, model.Quantity))
                {
                    Console.WriteLine($"AddToCart - Insufficient stock. Available: {product.StockQuantity}, Requested: {model.Quantity}");
                    return Json(new { success = false, message = $"Only {product.StockQuantity} items available" });
                }

                var buyerId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(buyerId))
                {
                    Console.WriteLine("AddToCart - User ID not found in session");
                    return Json(new { success = false, message = "Session expired" });
                }

                Console.WriteLine($"AddToCart - Calling EnhancedCartService.AddToCartAsync with BuyerId: '{buyerId}', ProductId: '{product.Id}', Quantity: {model.Quantity}");

                // Add to cart using EnhancedCartService
                var cartItem = await _enhancedCartService.AddToCartAsync(buyerId, product.Id, model.Quantity);
                
                if (cartItem == null)
                {
                    Console.WriteLine("AddToCart - Failed to add item to cart - service returned null");
                    return Json(new { success = false, message = "Failed to add product to cart" });
                }

                Console.WriteLine($"AddToCart - Successfully added to cart: {model.Quantity} x {product.Name}");
                
                // Get updated cart count
                var cartItems = await _enhancedCartService.GetCartItemsAsync(buyerId);
                var cartCount = cartItems.Sum(c => c.Quantity);

                Console.WriteLine($"AddToCart - Updated cart count: {cartCount}");

                return Json(new { 
                    success = true, 
                    message = $"Added {model.Quantity} {product.Name} to cart",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in AddToCart: {ex.Message}");
                Console.WriteLine($"ERROR Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, message = "An error occurred while adding to cart" });
            }
        }public async Task<IActionResult> Cart()
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            var buyerId = HttpContext.Session.GetString("UserId")!;
            
            try
            {
                // Use EnhancedCartService to get cart items with populated product data
                var cartItems = await _enhancedCartService.GetCartItemsWithProductsAsync(buyerId);

                // Calculate totals for ViewBag
                var subtotal = cartItems.Sum(c => c.Price * c.Quantity);
                var tax = subtotal * 0.08m; // 8% tax
                var total = subtotal + tax;

                ViewBag.Subtotal = subtotal.ToString("F2");
                ViewBag.Tax = tax.ToString("F2");
                ViewBag.Total = total.ToString("F2");

                return View(cartItems);
            }
            catch (Exception)
            {
                // Return empty cart on error
                ViewBag.Subtotal = "0.00";
                ViewBag.Tax = "0.00";
                ViewBag.Total = "0.00";
                return View(new List<CartItem>());
            }
        }        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(string cartItemId, int quantity)
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            await _enhancedCartService.UpdateCartItemAsync(cartItemId, quantity);
            return RedirectToAction("Cart");
        }        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string cartItemId)
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            await _enhancedCartService.RemoveFromCartAsync(cartItemId);
            TempData["Success"] = "Item removed from cart!";
            return RedirectToAction("Cart");
        }        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            var buyerId = HttpContext.Session.GetString("UserId")!;
            var cartItems = await _enhancedCartService.GetCartItemsWithProductsAsync(buyerId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Products");
            }

            var buyer = await _userService.GetBuyerByIdAsync(buyerId);
            var viewModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(c => c.Price * c.Quantity),
                FullName = $"{buyer!.FirstName} {buyer.LastName}",
                Email = buyer.Email,
                Phone = buyer.PhoneNumber,
                Address = buyer.Address,
                City = buyer.City,
                ZipCode = buyer.ZipCode
            };

            return View(viewModel);
        }        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            var buyerId = HttpContext.Session.GetString("UserId")!;
            var cartItems = await _enhancedCartService.GetCartItemsWithProductsAsync(buyerId);

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Products");
            }

            var deliveryAddress = $"{model.Address}, {model.City}, {model.State}, {model.ZipCode}";
            var order = await _orderService.CreateOrderAsync(buyerId, cartItems, deliveryAddress, model.Notes);
            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("OrderDetails", new { id = order.Id });
        }

        public async Task<IActionResult> Orders()
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            var buyerId = HttpContext.Session.GetString("UserId")!;
            var orders = await _orderService.GetOrdersByBuyerAsync(buyerId);

            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                UserType = "Buyer"
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(string id)
        {
            if (!IsValidBuyer())
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            var buyerId = HttpContext.Session.GetString("UserId")!;

            if (order == null || order.BuyerId != buyerId)
                return NotFound();

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                UserType = "Buyer",
                CanUpdateStatus = false
            };

            return View(viewModel);
        }        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity([FromBody] UpdateCartQuantityRequest request)
        {
            if (!IsValidBuyer())
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var success = await _enhancedCartService.UpdateCartItemAsync(request.ItemId, request.Quantity);
                
                if (success)
                {
                    // Get updated cart item to calculate new total
                    var buyerId = HttpContext.Session.GetString("UserId")!;
                    var cartItems = await _enhancedCartService.GetCartItemsWithProductsAsync(buyerId);
                    var updatedItem = cartItems.FirstOrDefault(c => c.Id == request.ItemId);
                    
                    var itemTotal = updatedItem != null ? 
                        (updatedItem.Quantity * updatedItem.Price).ToString("F2") : "0.00";
                    
                    return Json(new { success = true, itemTotal = itemTotal });
                }
                
                return Json(new { success = false, message = "Failed to update quantity" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error updating quantity" });
            }
        }        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            if (!IsValidBuyer())
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var success = await _enhancedCartService.RemoveFromCartAsync(request.ItemId);
                return Json(new { success = success, message = success ? "Item removed" : "Failed to remove item" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error removing item" });
            }
        }        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            if (!IsValidBuyer())
                return Json(new { success = false, message = "Unauthorized" });

            try
            {
                var buyerId = HttpContext.Session.GetString("UserId")!;
                var success = await _enhancedCartService.ClearCartAsync(buyerId);
                return Json(new { success = success });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error clearing cart" });
            }
        }        [HttpGet]
        public async Task<IActionResult> GetCartSummary()
        {
            if (!IsValidBuyer())
                return Json(new { subtotal = "0.00", tax = "0.00", total = "0.00" });

            try
            {
                var buyerId = HttpContext.Session.GetString("UserId")!;
                var cartItems = await _enhancedCartService.GetCartItemsWithProductsAsync(buyerId);
                
                var subtotal = cartItems.Sum(c => c.Price * c.Quantity);
                var tax = subtotal * 0.08m; // 8% tax
                var total = subtotal + tax;
                
                return Json(new 
                { 
                    subtotal = subtotal.ToString("F2"), 
                    tax = tax.ToString("F2"), 
                    total = total.ToString("F2") 
                });
            }
            catch (Exception)
            {
                return Json(new { subtotal = "0.00", tax = "0.00", total = "0.00" });
            }
        }        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!IsValidBuyer())
                return Json(new { count = 0 });

            try
            {
                var buyerId = HttpContext.Session.GetString("UserId")!;
                var count = await _enhancedCartService.GetCartItemCountAsync(buyerId);
                return Json(new { count = count });
            }
            catch (Exception)
            {
                return Json(new { count = 0 });
            }
        }

        // Request models for API endpoints
        public class UpdateCartQuantityRequest
        {
            public string ItemId { get; set; } = string.Empty;
            public int Quantity { get; set; }
        }

        public class RemoveFromCartRequest
        {
            public string ItemId { get; set; } = string.Empty;
        }

        private bool IsValidBuyer()
        {
            var userType = HttpContext.Session.GetString("UserType");
            var userId = HttpContext.Session.GetString("UserId");
            return userType == "Buyer" && !string.IsNullOrEmpty(userId);
        }
    }
}
