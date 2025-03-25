using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AltLogicViaPythonScript.Models
{
    internal class PurchaseOrder
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public required string CustomerPoNo { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal AdjustedOrderTotal { get; set; }

        public override string ToString() => $"PO# {CustomerPoNo}";
    }
}
