using MongoDB.Driver;
using FoxholeToolsAPI.Models;

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});
var app = builder.Build();

string StockpileCollection = DotEnv.config.GetValue<string>("DB_STOCKPILE_COLLECTION")!;

app.MapGet("/", () => "Testing");
app.MapGet("/stockpiles", async () => {
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection!);
    var results = await stockpileCollection.FindAsync(_ => true);
    var newRes = Results.Json(results.ToList());
    return newRes;
});
app.MapPut("/stockpiles", async (HttpRequest request) => {
    StockpileModel stockpile = new StockpileModel(await DBUtils.GetRequestJObject(request));
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    await stockpileCollection.InsertOneAsync(stockpile);

    return Results.Created($"/stockpiles/{stockpile.Id}", stockpile);
});
app.MapPut("/stockpiles/{id}", async (string id, HttpRequest request) => {
    StockpileModel stockpile = new StockpileModel(id, await DBUtils.GetRequestJObject(request));
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
    await stockpileCollection.ReplaceOneAsync(filter, stockpile);

    return Results.NoContent();
});
app.MapDelete("/stockpiles/delete/{id}", async (string id) => {
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
    await stockpileCollection.DeleteOneAsync(filter);

    return Results.NoContent();
});

app.Run();