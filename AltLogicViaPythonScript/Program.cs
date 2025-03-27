using AltLogicViaPythonScript.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Python.Runtime;

const string DB_NAME = "OrderSystem";
const string COLL_NAME = "PurchaseOrders";

var client = new MongoClient(GetMongoDbUri());
var collection = client.GetDatabase(DB_NAME).GetCollection<PurchaseOrder>(COLL_NAME);

var pos = new List<PurchaseOrder>()
{
    new() {CustomerPoNo = "ABC123", OrderTotal = 100, CustomProcessLogic = true },
    new() {CustomerPoNo = "DEF456", OrderTotal = 200, CustomProcessLogic = false },
    new() {CustomerPoNo = "GHI789", OrderTotal = 50, CustomProcessLogic = false }
};

var tasks = pos.Select(po => InsertAndUpdateOrder(collection, po));
await Task.WhenAll(tasks);

static async Task InsertAndUpdateOrder(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{
    await InsertPurchaseOrder(collection, po);

    if (!po.CustomProcessLogic)
        await StandardProcess(collection, po);
    else
        CustomProcess(collection, po);
}

static async Task<ObjectId> InsertPurchaseOrder(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{
    await collection.InsertOneAsync(po);
    return po.Id;
}

static async Task<bool> StandardProcess(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{
    var filter = Builders<PurchaseOrder>.Filter
        .Eq(p => p.Id, po.Id);

    // Standard process is to copy order total to adjusted order total
    var pipeline = new EmptyPipelineDefinition<PurchaseOrder>()
        .AppendStage<PurchaseOrder, PurchaseOrder, PurchaseOrder>("{ $set: {AdjustedOrderTotal: '$OrderTotal'} }");

    var update = Builders<PurchaseOrder>.Update.Pipeline(pipeline);

    var result = await collection.UpdateOneAsync(filter, update);

    return result.IsAcknowledged;
}

static bool CustomProcess(IMongoCollection < PurchaseOrder > collection, PurchaseOrder po)
{
    //TODO: Handle python dll location better
    Runtime.PythonDLL = "C:\\Users\\scott\\AppData\\Local\\Programs\\Python\\Python313\\python313.dll";

    PythonEngine.Initialize();
    PythonEngine.BeginAllowThreads();                   // Program hangs w/o this after executing Python script

    var gil = Py.GIL();

    try
    {
        dynamic module = Py.Import("CustomProcess");
        return module.customProcess(GetMongoDbUri(), po.Id.ToString());
    }
    catch (Exception ex)
    {
        Console.WriteLine(new string('-', 100));
        Console.WriteLine($"Exception occured running custom work for PO# {po.CustomerPoNo}: {ex.Message}.");
        Console.WriteLine(new string('-', 100));

        return false;
    }
    finally
    {
        gil.Dispose();
    }
}

static string GetMongoDbUri()
{
    IConfigurationRoot config = new ConfigurationBuilder()
        .AddUserSecrets<Program>()
        .Build();

    return config["MongoDbUri"] ?? throw new Exception("MongoDbUri value must be set in secrets.json.");
}
