using MongoDB.Bson.Serialization.Attributes;

namespace Farms.Models
{
    [BsonCollection("farmers")]
    public class Farmer : User
    {
        [BsonElement("farmName")]
        public string FarmName { get; set; } = string.Empty;

        [BsonElement("farmLocation")]
        public string FarmLocation { get; set; } = string.Empty;

        [BsonElement("farmSize")]
        public string FarmSize { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        public Farmer()
        {
            UserType = "Farmer";
        }
    }
}
