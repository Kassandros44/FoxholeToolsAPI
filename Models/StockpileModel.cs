using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FoxholeToolsAPI.Models;

public class StockpileModel{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id;
    public string name = string.Empty;
    public string location = string.Empty;
    public string passcode = string.Empty;
    public List<CrateModel> crates = new List<CrateModel>();
    public List<LogModel>? logs = new List<LogModel>();

    public StockpileModel(JObject jobject){
        
        Id = string.Empty;

        if(jobject.ContainsKey("name")){
            name = (string)jobject["name"]!;
        }
        if(jobject.ContainsKey("location")){
            location = (string)jobject["location"]!;
        }
        if(jobject.ContainsKey("passcode")){
            passcode = (string)jobject["passcode"]!;
        }
        if(jobject.ContainsKey("crates")){
            JArray jArray = (JArray)jobject["crates"]!;
            foreach(var i in jArray){
                JObject item = JObject.Parse(i.ToString());
                CrateModel crate = new CrateModel(item);
                crates!.Add(crate);
            }
        }
        if(jobject.ContainsKey("logs")){
            JArray jArray = (JArray)jobject["logs"]!;
            foreach(var i in jArray){
                JObject item = JObject.Parse(i.ToString());
                LogModel log = new LogModel(item);
                logs!.Add(log);
            }
        }
    }

        public StockpileModel(string id, JObject jobject){
        
        Id = id;

        if(jobject.ContainsKey("name")){
            name = (string)jobject["name"]!;
        }
        if(jobject.ContainsKey("location")){
            location = (string)jobject["location"]!;
        }
        if(jobject.ContainsKey("passcode")){
            passcode = (string)jobject["passcode"]!;
        }
        if(jobject.ContainsKey("crates")){
            JArray jArray = (JArray)jobject["crates"]!;
            foreach(var i in jArray){
                JObject item = JObject.Parse(i.ToString());
                CrateModel crate = new CrateModel(item);
                crates!.Add(crate);
            }
        }
        if(jobject.ContainsKey("logs")){
            JArray jArray = (JArray)jobject["logs"]!;
            foreach(var i in jArray){
                JObject item = JObject.Parse(i.ToString());
                LogModel log = new LogModel(item);
                logs!.Add(log);
            }
        }
    }

}