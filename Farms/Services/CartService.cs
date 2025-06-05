using MongoDB.Driver;
using Farms.Models;
using Farms.Data;

namespace Farms.Services
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartItemsAsync(string buyerId);
        Task<CartItem?> AddToCartAsync(string buyerId, string productId, int quantity);
        Task<bool> UpdateCartItemAsync(string cartItemId, int quantity);
        Task<bool> RemoveFromCartAsync(string cartItemId);
        Task<bool> ClearCartAsync(string buyerId);
        Task<decimal> GetCartTotalAsync(string buyerId);
        Task<int> GetCartItemCountAsync(string buyerId);
    }

    public class CartService : ICartService
    {
        private readonly MongoDbContext _context;
        private readonly IProductService _productService;

        public CartService(MongoDbContext context, IProductService productService)
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

        public async Task<CartItem?> AddToCartAsync(string buyerId, string productId, int quantity)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || !product.IsAvailable || product.Quantity < quantity)
                return null;

            // Check if item already exists in cart
            var existingCartItem = await _context.CartItems
                .Find(c => c.BuyerId == buyerId && c.ProductId == productId)
                .FirstOrDefaultAsync();

            if (existingCartItem != null)
            {
                // Update quantity
                existingCartItem.Quantity += quantity;
                if (existingCartItem.Quantity > product.Quantity)
                    existingCartItem.Quantity = product.Quantity;

                await _context.CartItems.ReplaceOneAsync(c => c.Id == existingCartItem.Id, existingCartItem);
                return existingCartItem;
            }
            else
            {
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
                    AddedAt = DateTime.UtcNow
                };

                await _context.CartItems.InsertOneAsync(cartItem);
                return cartItem;
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

            var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
            if (product == null || quantity > product.Quantity)
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
