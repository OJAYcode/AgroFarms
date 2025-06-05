using MongoDB.Driver;
using Farms.Models;
using Farms.Data;

namespace Farms.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task<List<Product>> GetProductsByFarmerAsync(string farmerId);
        Task<Product?> GetProductByIdAsync(string id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(string id);
        Task<List<string>> GetCategoriesAsync();
        Task<List<Product>> SearchProductsAsync(string searchTerm);
    }

    public class ProductService : IProductService
    {
        private readonly MongoDbContext _context;

        public ProductService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Find(p => p.IsAvailable && p.Quantity > 0)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Find(p => p.Category == category && p.IsAvailable && p.Quantity > 0)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByFarmerAsync(string farmerId)
        {
            return await _context.Products
                .Find(p => p.FarmerId == farmerId)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(string id)
        {
            return await _context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var result = await _context.Products
                .ReplaceOneAsync(p => p.Id == product.Id, product);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var result = await _context.Products
                .DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }        public async Task<List<string>> GetCategoriesAsync()
        {
            var filter = Builders<Product>.Filter.Eq(p => p.IsAvailable, true);
            var categories = await _context.Products
                .Distinct<string>("category", filter)
                .ToListAsync();
            
            return categories.Where(c => !string.IsNullOrEmpty(c)).ToList();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.IsAvailable, true),
                Builders<Product>.Filter.Gt(p => p.Quantity, 0),
                Builders<Product>.Filter.Or(
                    Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<Product>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                    Builders<Product>.Filter.Regex(p => p.Category, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
                )
            );

            return await _context.Products
                .Find(filter)
                .SortByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
