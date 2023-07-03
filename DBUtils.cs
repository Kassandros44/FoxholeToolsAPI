using MongoDB.Driver;

public static class DBUtils{

    private static string? connectionURI = DotEnv.config.GetValue<string>("DB_CONNECTION_URI");
    private static string? databaseName = DotEnv.config.GetValue<string>("DB_NAME");

    public static IMongoCollection<T> ConnectToMongo<T>(in string collection){
        var client = new MongoClient(connectionURI);
        var db = client.GetDatabase(databaseName);
        return db.GetCollection<T>(collection);
    }

    public static async Task<JObject> GetRequestJObject(HttpRequest request){
        var body = new StreamReader(request.Body);
        string putData = await  body.ReadToEndAsync();
        JObject jobject = JObject.Parse(putData);
        return jobject;
    }
}