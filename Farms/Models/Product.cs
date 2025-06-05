using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Farms.Models
{
    [BsonCollection("products")]
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unit")]
        public string Unit { get; set; } = string.Empty; // kg, tons, pieces, etc.

        [BsonElement("farmerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FarmerId { get; set; } = string.Empty;

        [BsonElement("farmerName")]
        public string FarmerName { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; } = true;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;        [BsonElement("harvestDate")]
        public DateTime? HarvestDate { get; set; }

        [BsonElement("expiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [BsonElement("isOrganic")]
        public bool IsOrganic { get; set; } = false;

        [BsonElement("stockQuantity")]
        public int StockQuantity { get; set; }
    }
}
