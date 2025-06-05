using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Farms.Models
{
    [BsonCollection("cart")]
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("buyerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuyerId { get; set; } = string.Empty;

        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("productName")]
        public string ProductName { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unit")]
        public string Unit { get; set; } = string.Empty;

        [BsonElement("farmerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FarmerId { get; set; } = string.Empty;

        [BsonElement("farmerName")]
        public string FarmerName { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;        [BsonElement("addedAt")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for views (not stored in MongoDB)
        [BsonIgnore]
        public Product? Product { get; set; }
    }
}
