using AltLogicViaPythonScript.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

const string DB_NAME = "OrderSystem";
const string COLL_NAME = "PurchaseOrders";

var uri = GetMongoDbUri();

var client = new MongoClient(uri);

var collection = client.GetDatabase(DB_NAME).GetCollection<PurchaseOrder>(COLL_NAME);


var po = new PurchaseOrder
{
    CustomerPoNo = "ABC123",
    LineItems =
    [
        new LineItem() {LineNo = 1, Description = "Cogs", QuantityOrdered = 10},
        new LineItem() {LineNo = 2, Description = "Widgets", QuantityOrdered = 20}
    ]
};

await collection.InsertOneAsync(po);


static string GetMongoDbUri()
{
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    string? uri = config["MongoDbUri"];

    if (uri == null)
        throw new Exception("MongoDbUri value must be set in secrets.json.");

    return uri;
}
