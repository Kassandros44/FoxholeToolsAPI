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

string StockpileCollection = "Stockpiles";
string UserCollection = "Users";
string Passkey = "82dkAppTest";

app.MapGet("/", () => "Testing");

//Check Passkey
app.MapGet("/checklogin/{passkey}", (string passkey) => {
    bool response;
    if(passkey != Passkey){
        response = false;
    }else{
        response = true;
    }
    return TypedResults.Ok<bool>(response);
});

//Get all Stockpiles
app.MapGet("/stockpiles", async () => {
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection!);
    var results = await stockpileCollection.FindAsync(_ => true);
    var newRes = Results.Json(results.ToList());
    return newRes;
});

//Put new stockpile
app.MapPut("/stockpiles/new", async (HttpRequest request) => {
    StockpileModel stockpile = new StockpileModel(await DBUtils.GetRequestJObject(request));
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    await stockpileCollection.InsertOneAsync(stockpile);

    return Results.Created($"/stockpiles/{stockpile.Id}", stockpile);
});

//Replace stockpile by id
app.MapPut("/stockpiles/update/{id}", async (string id, HttpRequest request) => {
    StockpileModel stockpile = new StockpileModel(id, await DBUtils.GetRequestJObject(request));
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
    await stockpileCollection.ReplaceOneAsync(filter, stockpile);

    return Results.NoContent();
});

//Delete stockpile by id
app.MapDelete("/stockpiles/delete/{id}", async (string id) => {
    var stockpileCollection = DBUtils.ConnectToMongo<StockpileModel>(StockpileCollection);
    var filter = Builders<StockpileModel>.Filter.Eq("Id", id);
    await stockpileCollection.DeleteOneAsync(filter);

    return Results.NoContent();
});

//Create User
app.MapPut("/users", async (HttpRequest request)=>{
    UserModel user = new UserModel(await DBUtils.GetRequestJObject(request));
    var userCollection = DBUtils.ConnectToMongo<UserModel>(UserCollection);
    await userCollection.InsertOneAsync(user);

    return Results.Created($"/users/{user.Id}", user);
});

//Find User
app.MapGet("/users/find/{username}", async (string username) => {
    var userCollection = DBUtils.ConnectToMongo<UserModel>(UserCollection);
    var builder = Builders<UserModel>.Filter;
        var filter = builder.Eq(u => u.username, username);
        var results = await userCollection.Find<UserModel>(filter).FirstOrDefaultAsync();
        bool response;
        if(results != null){
            response = true;
        }else{
            response = false;
        }
        return TypedResults.Ok<bool>(response);
});

//Get User
app.MapGet("/users/{username}", async (string username) => {
    var userCollection = DBUtils.ConnectToMongo<UserModel>(UserCollection);
    var builder = Builders<UserModel>.Filter;
        var filter = builder.Eq(u => u.username, username);
        var results = await userCollection.Find<UserModel>(filter).FirstOrDefaultAsync();
        return results;
});

app.Run();