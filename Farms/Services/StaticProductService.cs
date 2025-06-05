using Farms.Models;

namespace Farms.Services
{
    public interface IStaticProductService
    {
        List<Product> GetAllProducts();
        Product? GetProductById(string productId);
        bool IsProductAvailable(string productId, int quantity = 1);
        List<Product> FilterProducts(IEnumerable<Product> products, string? category = null, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, bool? isOrganic = null, string? sortBy = null);
    }

    public class StaticProductService : IStaticProductService
    {
        private readonly List<Product> _staticProducts;        public StaticProductService()
        {
            _staticProducts = GetStaticProducts();
        }

        public List<Product> GetAllProducts()
        {
            return _staticProducts.Where(p => p.IsAvailable).ToList();
        }        public Product? GetProductById(string productId)
        {
            var product = _staticProducts.FirstOrDefault(p => p.Id == productId && p.IsAvailable);
            return product;
        }

        public bool IsProductAvailable(string productId, int quantity)
        {
            var product = GetProductById(productId);
            return product != null && product.StockQuantity >= quantity;
        }        public List<Product> FilterProducts(IEnumerable<Product> products, string? category = null, string? searchTerm = null, decimal? minPrice = null, decimal? maxPrice = null, bool? isOrganic = null, string? sortBy = null)
        {
            var filteredProducts = products.AsQueryable();

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                filteredProducts = filteredProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                filteredProducts = filteredProducts.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                             p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by price range
            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
            }

            // Filter by organic
            if (isOrganic.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.IsOrganic == isOrganic.Value);
            }

            // Sort products
            switch (sortBy?.ToLower())
            {
                case "price_asc":
                    filteredProducts = filteredProducts.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    filteredProducts = filteredProducts.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Name);
                    break;
                case "newest":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    filteredProducts = filteredProducts.OrderBy(p => p.Name);
                    break;
            }

            return filteredProducts.ToList();
        }

        private List<Product> GetStaticProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = "1",
                    Name = "Organic Tomatoes",
                    Description = "Fresh, juicy organic tomatoes grown without pesticides. Perfect for salads, cooking, and sauces.",
                    Price = 4.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer1",
                    FarmerName = "Green Valley Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1546470427-e26264ce582f?w=400",
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
                    Name = "Sweet Potatoes",
                    Description = "Orange-fleshed sweet potatoes with natural sweetness. High in vitamins A and C.",
                    Price = 2.99m,
                    Category = "Vegetables",
                    Unit = "lb",
                    FarmerId = "farmer4",
                    FarmerName = "Harvest Moon Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1586190848861-99aa4a171e90?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    IsOrganic = true,
                    StockQuantity = 40
                },
                new Product
                {
                    Id = "6",
                    Name = "Spinach Leaves",
                    Description = "Fresh baby spinach leaves, perfect for salads or cooking. Rich in iron and nutrients.",
                    Price = 3.99m,
                    Category = "Vegetables",
                    Unit = "bunch",
                    FarmerId = "farmer2",
                    FarmerName = "Sunset Organic Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1576045057995-568f588f82fb?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsOrganic = true,
                    StockQuantity = 22
                },
                new Product
                {
                    Id = "7",
                    Name = "Red Apples",
                    Description = "Crisp and sweet red apples, perfect for snacking or baking. Freshly picked from our orchard.",
                    Price = 3.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer5",
                    FarmerName = "Orchard Hill Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1560806887-1e4cd0b6cbd6?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    IsOrganic = false,
                    StockQuantity = 35
                },
                new Product
                {
                    Id = "8",
                    Name = "Bananas",
                    Description = "Ripe yellow bananas, naturally sweet and perfect for breakfast or smoothies.",
                    Price = 2.49m,
                    Category = "Fruits",
                    Unit = "bunch",
                    FarmerId = "farmer6",
                    FarmerName = "Tropical Grove",
                    ImageUrl = "https://images.unsplash.com/photo-1571771894821-ce9b6c11b08e?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    IsOrganic = true,
                    StockQuantity = 28
                },
                new Product
                {
                    Id = "9",
                    Name = "Strawberries",
                    Description = "Sweet, juicy strawberries bursting with flavor. Perfect for desserts or eating fresh.",
                    Price = 5.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer7",
                    FarmerName = "Berry Patch Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1464965911861-746a04b4bca6?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsOrganic = true,
                    StockQuantity = 15
                },
                new Product
                {
                    Id = "10",
                    Name = "Blueberries",
                    Description = "Antioxidant-rich blueberries, sweet and perfect for muffins, pancakes, or snacking.",
                    Price = 7.99m,
                    Category = "Fruits",
                    Unit = "lb",
                    FarmerId = "farmer7",
                    FarmerName = "Berry Patch Farm",
                    ImageUrl = "https://images.unsplash.com/photo-1498557850523-fd3d118b962e?w=400",
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    IsOrganic = true,
                    StockQuantity = 12
                }
            };
        }
    }
}
