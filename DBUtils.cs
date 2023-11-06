using MongoDB.Driver;

public static class DBUtils{

    private static string? connectionURI = "mongodb+srv://FoxholeToolsApi:8H3iD9MzymIre4HA@foxholetools.rcw6ivk.mongodb.net/";
    private static string? databaseName = "FoxholeTools";

    public static IMongoCollection<T> ConnectToMongo<T>(in string collection){
        var client = new MongoClient(connectionURI);
        var db = client.GetDatabase(databaseName);
        return db.GetCollection<T>(collection);
    }

    public static async Task<JObject> GetRequestJObject(HttpRequest request){
        var body = new StreamReader(request.Body);
        string putData = await body.ReadToEndAsync();
        JObject jobject = JObject.Parse(putData);
        return jobject;
    }

}