using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Farms.Models
{
    [BsonCollection("orders")]
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("buyerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuyerId { get; set; } = string.Empty;

        [BsonElement("buyerName")]
        public string BuyerName { get; set; } = string.Empty;

        [BsonElement("buyerEmail")]
        public string BuyerEmail { get; set; } = string.Empty;

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }

        [BsonElement("status")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("deliveryAddress")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [BsonElement("shippingAddress")]
        public string ShippingAddress { get; set; } = string.Empty;

        [BsonElement("notes")]
        public string Notes { get; set; } = string.Empty;
    }

    public class OrderItem
    {
        [BsonElement("productId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; } = string.Empty;

        [BsonElement("productName")]
        public string ProductName { get; set; } = string.Empty;

        [BsonElement("farmerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FarmerId { get; set; } = string.Empty;

        [BsonElement("farmerName")]
        public string FarmerName { get; set; } = string.Empty;

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }        [BsonElement("totalPrice")]
        public decimal TotalPrice { get; set; }

        [BsonElement("unit")]
        public string Unit { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("isOrganic")]
        public bool IsOrganic { get; set; } = false;
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
