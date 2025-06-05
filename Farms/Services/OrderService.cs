using MongoDB.Driver;
using Farms.Models;
using Farms.Data;

namespace Farms.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerId, List<CartItem> cartItems, string deliveryAddress, string notes = "");
        Task<List<Order>> GetOrdersByBuyerAsync(string buyerId);
        Task<List<Order>> GetOrdersByFarmerAsync(string farmerId);
        Task<Order?> GetOrderByIdAsync(string orderId);
        Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status);
        Task<List<Order>> GetAllOrdersAsync();
    }

    public class OrderService : IOrderService
    {
        private readonly MongoDbContext _context;
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public OrderService(MongoDbContext context, ICartService cartService, IProductService productService)
        {
            _context = context;
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<Order> CreateOrderAsync(string buyerId, List<CartItem> cartItems, string deliveryAddress, string notes = "")
        {
            var buyer = await _context.Buyers.Find(b => b.Id == buyerId).FirstOrDefaultAsync();
            if (buyer == null) throw new ArgumentException("Buyer not found");

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var cartItem in cartItems)
            {
                // Verify product availability and update stock
                var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
                if (product == null || product.Quantity < cartItem.Quantity)
                    continue; // Skip unavailable products

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    FarmerId = cartItem.FarmerId,
                    FarmerName = cartItem.FarmerName,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Price,
                    TotalPrice = cartItem.Price * cartItem.Quantity,
                    Unit = cartItem.Unit
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;

                // Update product quantity
                product.Quantity -= cartItem.Quantity;
                await _productService.UpdateProductAsync(product);
            }

            var order = new Order
            {
                BuyerId = buyerId,
                BuyerName = $"{buyer.FirstName} {buyer.LastName}",
                BuyerEmail = buyer.Email,
                Items = orderItems,
                TotalAmount = totalAmount,
                DeliveryAddress = deliveryAddress,
                Notes = notes,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending
            };

            await _context.Orders.InsertOneAsync(order);

            // Clear cart after successful order
            await _cartService.ClearCartAsync(buyerId);

            return order;
        }

        public async Task<List<Order>> GetOrdersByBuyerAsync(string buyerId)
        {
            return await _context.Orders
                .Find(o => o.BuyerId == buyerId)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByFarmerAsync(string farmerId)
        {
            var filter = Builders<Order>.Filter.ElemMatch(o => o.Items, 
                Builders<OrderItem>.Filter.Eq(i => i.FarmerId, farmerId));

            return await _context.Orders
                .Find(filter)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Find(o => o.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, status);
            var result = await _context.Orders.UpdateOneAsync(o => o.Id == orderId, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Find(_ => true)
                .SortByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
