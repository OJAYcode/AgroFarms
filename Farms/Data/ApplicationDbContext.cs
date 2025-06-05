using MongoDB.Driver;
using Farms.Models;
using Microsoft.Extensions.Options;

namespace Farms.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Farmer> Farmers => _database.GetCollection<Farmer>("farmers");
        public IMongoCollection<Buyer> Buyers => _database.GetCollection<Buyer>("buyers");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
        public IMongoCollection<CartItem> CartItems => _database.GetCollection<CartItem>("cart");

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
