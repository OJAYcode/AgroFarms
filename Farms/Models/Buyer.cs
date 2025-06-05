using MongoDB.Bson.Serialization.Attributes;

namespace Farms.Models
{
    [BsonCollection("buyers")]
    public class Buyer : User
    {
        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("zipCode")]
        public string ZipCode { get; set; } = string.Empty;

        [BsonElement("company")]
        public string Company { get; set; } = string.Empty;

        public Buyer()
        {
            UserType = "Buyer";
        }
    }
}
