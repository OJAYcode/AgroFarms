using MongoDB.Driver;
using Farms.Models;
using Farms.Data;

namespace Farms.Services
{
    public interface IEnhancedCartService
    {
        Task<List<CartItem>> GetCartItemsAsync(string buyerId);
        Task<CartItem?> AddToCartAsync(string buyerId, string productId, int quantity);
        Task<bool> UpdateCartItemAsync(string cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(string cartItemId);
        Task<bool> ClearCartAsync(string buyerId);
        Task<decimal> GetCartTotalAsync(string buyerId);
        Task<int> GetCartItemCountAsync(string buyerId);
        Task<List<CartItem>> GetCartItemsWithProductsAsync(string buyerId);
    }

    public class EnhancedCartService : IEnhancedCartService
    {
        private readonly MongoDbContext _context;
        private readonly IStaticProductService _productService;

        public EnhancedCartService(MongoDbContext context, IStaticProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string buyerId)
        {
            return await _context.CartItems
                .Find(c => c.BuyerId == buyerId)
                .SortByDescending(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<List<CartItem>> GetCartItemsWithProductsAsync(string buyerId)
        {
            var cartItems = await GetCartItemsAsync(buyerId);
            
            // Populate product data for each cart item
            foreach (var item in cartItems)
            {
                item.Product = _productService.GetProductById(item.ProductId);
            }

            return cartItems;
        }        public async Task<CartItem?> AddToCartAsync(string buyerId, string productId, int quantity)
        {
            Console.WriteLine($"AddToCartAsync called - BuyerId: '{buyerId}', ProductId: '{productId}', Quantity: {quantity}");
            
            var product = _productService.GetProductById(productId);
            if (product == null || !product.IsAvailable)
            {
                Console.WriteLine($"AddToCartAsync - Product not found or not available. ProductId: '{productId}'");
                return null;
            }

            if (!_productService.IsProductAvailable(productId, quantity))
            {
                Console.WriteLine($"AddToCartAsync - Insufficient stock. ProductId: '{productId}', Requested: {quantity}, Available: {product.StockQuantity}");
                return null;
            }

            try
            {
                // Check if item already exists in cart
                var existingCartItem = await _context.CartItems
                    .Find(c => c.BuyerId == buyerId && c.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (existingCartItem != null)
                {
                    Console.WriteLine($"AddToCartAsync - Updating existing cart item. ItemId: '{existingCartItem.Id}'");
                    
                    // Update quantity
                    var newQuantity = existingCartItem.Quantity + quantity;
                    if (newQuantity > product.StockQuantity)
                        newQuantity = product.StockQuantity;

                    existingCartItem.Quantity = newQuantity;
                    await _context.CartItems.ReplaceOneAsync(c => c.Id == existingCartItem.Id, existingCartItem);
                    
                    // Populate product data
                    existingCartItem.Product = product;
                    return existingCartItem;
                }
                else
                {
                    Console.WriteLine($"AddToCartAsync - Creating new cart item for product: '{product.Name}'");
                    
                    // Create new cart item
                    var cartItem = new CartItem
                    {
                        BuyerId = buyerId,
                        ProductId = productId,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = quantity,
                        Unit = product.Unit,
                        FarmerId = product.FarmerId,
                        FarmerName = product.FarmerName,
                        ImageUrl = product.ImageUrl,
                        AddedAt = DateTime.UtcNow,
                        Product = product
                    };

                    await _context.CartItems.InsertOneAsync(cartItem);
                    Console.WriteLine($"AddToCartAsync - Successfully created cart item. ItemId: '{cartItem.Id}'");
                    return cartItem;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddToCartAsync - Exception: {ex.Message}");
                Console.WriteLine($"AddToCartAsync - Stack trace: {ex.StackTrace}");
                throw; // Re-throw to let the controller handle it
            }
        }

        public async Task<bool> UpdateCartItemAsync(string cartItemId, int quantity)
        {
            if (quantity <= 0)
                return await RemoveFromCartAsync(cartItemId);

            var cartItem = await _context.CartItems
                .Find(c => c.Id == cartItemId)
                .FirstOrDefaultAsync();

            if (cartItem == null) return false;

            var product = _productService.GetProductById(cartItem.ProductId);
            if (product == null || quantity > product.StockQuantity)
                return false;

            cartItem.Quantity = quantity;
            var result = await _context.CartItems.ReplaceOneAsync(c => c.Id == cartItemId, cartItem);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveFromCartAsync(string cartItemId)
        {
            var result = await _context.CartItems.DeleteOneAsync(c => c.Id == cartItemId);
            return result.DeletedCount > 0;
        }

        public async Task<bool> ClearCartAsync(string buyerId)
        {
            await _context.CartItems.DeleteManyAsync(c => c.BuyerId == buyerId);
            return true;
        }

        public async Task<decimal> GetCartTotalAsync(string buyerId)
        {
            var cartItems = await GetCartItemsAsync(buyerId);
            return cartItems.Sum(c => c.Price * c.Quantity);
        }

        public async Task<int> GetCartItemCountAsync(string buyerId)
        {
            var count = await _context.CartItems
                .CountDocumentsAsync(c => c.BuyerId == buyerId);
            return (int)count;
        }
    }
}
