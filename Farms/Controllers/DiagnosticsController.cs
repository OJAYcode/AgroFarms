using Microsoft.AspNetCore.Mvc;
using Farms.Data;
using Farms.Models;
using MongoDB.Driver;
using System.Diagnostics;

namespace Farms.Controllers
{
    public class DiagnosticsController : Controller
    {
        private readonly MongoDbContext _context;

        public DiagnosticsController(MongoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult TestMongoDB()
        {
            try
            {
                // Test connection to MongoDB
                var databaseName = _context.GetType().GetProperty("DatabaseName")?.GetValue(_context) ?? "Unknown";
                var collections = _context.GetType().GetProperties()
                    .Where(p => p.PropertyType.IsGenericType && 
                           p.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>))
                    .Select(p => p.Name)
                    .ToList();

                // Test writing to cart collection
                var testItem = new CartItem
                {
                    Id = "test-" + Guid.NewGuid().ToString(),
                    BuyerId = "test-buyer",
                    ProductId = "test-product",
                    ProductName = "Test Product",
                    Price = 9.99m,
                    Quantity = 1,
                    Unit = "item",
                    AddedAt = DateTime.UtcNow
                };

                _context.CartItems.InsertOne(testItem);

                // Test reading from cart collection
                var filter = Builders<CartItem>.Filter.Eq(c => c.BuyerId, "test-buyer");
                var testItems = _context.CartItems.Find(filter).ToList();

                // Cleanup test data
                _context.CartItems.DeleteOne(c => c.Id == testItem.Id);

                return Json(new
                {
                    success = true,
                    message = "MongoDB connection test successful",
                    databaseName = databaseName,
                    collections = collections,
                    testItems = testItems.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "MongoDB connection test failed",
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }
    }
}
