using MongoDB.Driver;
using Farms.Models;
using Farms.Data;
using BCrypt.Net;

namespace Farms.Services
{
    public interface IUserService
    {
        Task<Farmer?> RegisterFarmerAsync(Farmer farmer, string password);
        Task<Buyer?> RegisterBuyerAsync(Buyer buyer, string password);
        Task<Farmer?> AuthenticateFarmerAsync(string email, string password);
        Task<Buyer?> AuthenticateBuyerAsync(string email, string password);
        Task<Farmer?> GetFarmerByIdAsync(string id);
        Task<Buyer?> GetBuyerByIdAsync(string id);
        Task<bool> EmailExistsAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly MongoDbContext _context;

        public UserService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var farmerExists = await _context.Farmers
                .Find(f => f.Email == email)
                .AnyAsync();

            if (farmerExists) return true;

            var buyerExists = await _context.Buyers
                .Find(b => b.Email == email)
                .AnyAsync();

            return buyerExists;
        }

        public async Task<Farmer?> RegisterFarmerAsync(Farmer farmer, string password)
        {
            if (await EmailExistsAsync(farmer.Email))
                return null;

            farmer.Password = BCrypt.Net.BCrypt.HashPassword(password);
            farmer.CreatedAt = DateTime.UtcNow;

            await _context.Farmers.InsertOneAsync(farmer);
            return farmer;
        }

        public async Task<Buyer?> RegisterBuyerAsync(Buyer buyer, string password)
        {
            if (await EmailExistsAsync(buyer.Email))
                return null;

            buyer.Password = BCrypt.Net.BCrypt.HashPassword(password);
            buyer.CreatedAt = DateTime.UtcNow;

            await _context.Buyers.InsertOneAsync(buyer);
            return buyer;
        }

        public async Task<Farmer?> AuthenticateFarmerAsync(string email, string password)
        {
            var farmer = await _context.Farmers
                .Find(f => f.Email == email)
                .FirstOrDefaultAsync();

            if (farmer == null || !BCrypt.Net.BCrypt.Verify(password, farmer.Password))
                return null;

            return farmer;
        }

        public async Task<Buyer?> AuthenticateBuyerAsync(string email, string password)
        {
            var buyer = await _context.Buyers
                .Find(b => b.Email == email)
                .FirstOrDefaultAsync();

            if (buyer == null || !BCrypt.Net.BCrypt.Verify(password, buyer.Password))
                return null;

            return buyer;
        }

        public async Task<Farmer?> GetFarmerByIdAsync(string id)
        {
            return await _context.Farmers
                .Find(f => f.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Buyer?> GetBuyerByIdAsync(string id)
        {
            return await _context.Buyers
                .Find(b => b.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
