﻿using AltLogicViaPythonScript.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

const string DB_NAME = "OrderSystem";
const string COLL_NAME = "PurchaseOrders";

var uri = GetMongoDbUri();
var client = new MongoClient(uri);
var collection = client.GetDatabase(DB_NAME).GetCollection<PurchaseOrder>(COLL_NAME);

var pos = new List<PurchaseOrder>()
{
    new() {CustomerPoNo = "ABC123", OrderTotal = 100 },
    new() {CustomerPoNo = "DEF456", OrderTotal = 200 },
    new() {CustomerPoNo = "GHI789", OrderTotal = 50 }
};

//pos.ForEach(async p => await InsertAndUpdateOrder(collection, p));

//foreach (var po in pos)
//{
//    await InsertPurchaseOrder(collection, po);
//    await UpdatePurchaseOrder(collection, po);
//}

var tasks = pos.Select(po => InsertAndUpdateOrder(collection, po));
await Task.WhenAll(tasks);

static async Task InsertAndUpdateOrder(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{
    await InsertPurchaseOrder(collection, po);
    await UpdatePurchaseOrder(collection, po);
}

static async Task<ObjectId> InsertPurchaseOrder(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{
    await collection.InsertOneAsync(po);
    return po.Id;
}

static async Task<bool> UpdatePurchaseOrder(IMongoCollection<PurchaseOrder> collection, PurchaseOrder po)
{

    var filter = Builders<PurchaseOrder>.Filter
        .Eq(p => p.Id, po.Id);

    // TODO: Replace with actual logic
    var update = Builders<PurchaseOrder>.Update
        .Set("AdjustedOrderTotal", 123);

    var result = await collection.UpdateOneAsync(filter, update);

    return result.IsAcknowledged;
}

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
