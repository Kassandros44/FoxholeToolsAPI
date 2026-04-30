using FoxholeToolsAPI.Models;
using FoxholeToolsAPI.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data;
using System.Runtime.InteropServices;

public class StockpileEndpoints
{
    public static void Map(WebApplication app)
    {
        //Get all Stockpiles
        app.MapGet("/stockpiles", async () => {
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var results = await stockpileCollection.FindAsync(_ => true);
            var newRes = Results.Json(results.ToList());
            return newRes;
        });

        //Get a struct of stockpile identity data
        app.MapGet("/stockpiles/simpledata", async ([FromHeader(Name = "Current-Stockpiles")] string currentStockpiles) => {

            List<StockpileModel.simpleData> simpleData = new List<StockpileModel.simpleData>();

            Console.WriteLine($"Current Data: {currentStockpiles}");

            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var results = await stockpileCollection.FindAsync(_ => true);
            var stockpileList = results.ToList();

            foreach (var stockpile in stockpileList)
            {
                simpleData.Add(stockpile.createSimple());
            }

            var returnResults = Results.Json(simpleData);
            return returnResults;
        });

        app.MapGet("/stockpile/crates/{id}", async (string id) =>
        {
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
            var stockpile = await stockpileCollection.Find(filter).FirstOrDefaultAsync();
            var results = Results.Json(stockpile.crates.ToList());

            return results;
        });

        app.MapGet("/stockpile/name/{id}", async (string id) =>
        {
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
            var stockpile = await stockpileCollection.Find(filter).FirstOrDefaultAsync();
            var results = Results.Json(stockpile.name);

            return results;
        });

        app.MapGet("/stockpile/initsync", async () =>
        {
            var _stockpiles = await DBUtils.ConnectToMongo<StockpileModel>("Stockpiles")
                .Find(_ => true)
                .ToListAsync();

            var activeStockpiles = _stockpiles
                .Where(x => !x.IsDeleted)
                .Select(x => new StockpileIdDto(x))
                .ToList();

            return Results.Ok(new
            {
                activeStockpiles
            });
        });

        app.MapGet("/stockpile/sync", async (
            DateTime lastSync) =>
        {
            var now = DateTime.UtcNow;

            var filter = Builders<StockpileModel>.Filter.And(
                Builders<StockpileModel>.Filter.Gt(x => x.UpdatedAt, lastSync),
                Builders<StockpileModel>.Filter.Lte(x => x.UpdatedAt, now)
            );

            var changedItems = await DBUtils.ConnectToMongo<StockpileModel>("Stockpiles")
                .Find(filter)
                .ToListAsync();

            var added = changedItems
                .Where(x => !x.IsDeleted)
                .Select(x => new StockpileIdDto(x))
                .ToList();

            var removed = changedItems
                .Where(x => x.IsDeleted)
                .Select(x => x._id.ToString())
                .ToList();

            return Results.Ok(new
            {
                added,
                removed,
                serverTime = now
            });
        });

        app.MapPut("/stockpiles/new", async (HttpRequest request) => {
            StockpileModel stockpile = new StockpileModel(await DBUtils.GetRequestJObject(request));
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            stockpile.UpdatedAt = DateTime.UtcNow;
            await stockpileCollection.InsertOneAsync(stockpile);

            return Results.Created($"/stockpiles/{stockpile._id}", stockpile);
        });

        app.MapPost("/stockpile/upsert", async (
            StockpileModel stockpile,
            IMongoCollection<StockpileModel> collection) =>
        {
            if(string.IsNullOrEmpty(stockpile._id))
                stockpile._id = ObjectId.GenerateNewId().ToString();

            stockpile.UpdatedAt = DateTime.UtcNow;
            stockpile.IsDeleted = false;

            var filter = Builders<StockpileModel>.Filter.Eq(x => x._id, stockpile._id);

            await collection.ReplaceOneAsync(
                filter,
                stockpile,
                new ReplaceOptions { IsUpsert = true }
            );

            return Results.Created($"/stockpiles/{stockpile._id}", stockpile);
        });

        app.MapPost("/stockpile/softdelete", async (
            string _id,
            IMongoCollection<StockpileModel> collection) =>
        {

            var filter = Builders<StockpileModel>.Filter.Eq(x => x._id, _id);

            var stp = collection.Find(filter).FirstOrDefault<StockpileModel>();
            stp.UpdatedAt = DateTime.UtcNow;
            stp.IsDeleted = true;

            await collection.ReplaceOneAsync(
                filter,
                stp,
                new ReplaceOptions { IsUpsert = true }
            );

            return Results.Ok($"{stp._id} :: deleteStatus {stp.IsDeleted}");

        });
        
        app.MapPut("/stockpiles/update/{id}", async (string id, HttpRequest request) => {
            StockpileModel stockpile = new StockpileModel(id, await DBUtils.GetRequestJObject(request));
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
            await stockpileCollection.ReplaceOneAsync(filter, stockpile);

            return Results.NoContent();
        });

        app.MapDelete("/stockpiles/delete/{id}", async (string id) => {
            var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>("Stockpiles");
            var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
            await stockpileCollection.DeleteOneAsync(filter);

            return Results.NoContent();
        });
    }
}
