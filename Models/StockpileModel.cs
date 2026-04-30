using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Discord.Rest;

namespace FoxholeToolsAPI.Models;

public class StockpileModel
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id { get; set; }
    public string name = string.Empty;
    public string region = string.Empty;
    public string location = string.Empty;
    public string passcode = string.Empty;
    public List<CrateModel> crates = new List<CrateModel>();
    public List<LogModel>? logs = new List<LogModel>();

    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public struct simpleData
    {
        public string Id;
        public string name;
        public string region;
        public string location;
        public string passcode;
    }

    public StockpileModel()
    {

    }

    public StockpileModel(JObject jobject)
    {

        _id = string.Empty;

        if (jobject.ContainsKey("name"))
        {
            name = (string)jobject["name"]!;
        }
        if (jobject.ContainsKey("region"))
        {
            region = (string)jobject["region"]!;
        }
        if (jobject.ContainsKey("location"))
        {
            location = (string)jobject["location"]!;
        }
        if (jobject.ContainsKey("passcode"))
        {
            passcode = (string)jobject["passcode"]!;
        }
        if (jobject.ContainsKey("crates"))
        {
            JArray jArray = (JArray)jobject["crates"]!;
            foreach (var i in jArray)
            {
                JObject item = JObject.Parse(i.ToString());
                CrateModel crate = new CrateModel(item);
                crates!.Add(crate);
            }
        }
        if (jobject.ContainsKey("logs"))
        {
            JArray jArray = (JArray)jobject["logs"]!;
            foreach (var i in jArray)
            {
                JObject item = JObject.Parse(i.ToString());
                LogModel log = new LogModel(item);
                logs!.Add(log);
            }
        }
    }

    public StockpileModel(string id, JObject jobject)
    {

        _id = id;

        if (jobject.ContainsKey("name"))
        {
            name = (string)jobject["name"]!;
        }
        if (jobject.ContainsKey("region"))
        {
            region = (string)jobject["region"]!;
        }
        if (jobject.ContainsKey("location"))
        {
            location = (string)jobject["location"]!;
        }
        if (jobject.ContainsKey("passcode"))
        {
            passcode = (string)jobject["passcode"]!;
        }
        if (jobject.ContainsKey("crates"))
        {
            JArray jArray = (JArray)jobject["crates"]!;
            foreach (var i in jArray)
            {
                JObject item = JObject.Parse(i.ToString());
                CrateModel crate = new CrateModel(item);
                crates!.Add(crate);
            }
        }
        if (jobject.ContainsKey("logs"))
        {
            JArray jArray = (JArray)jobject["logs"]!;
            foreach (var i in jArray)
            {
                JObject item = JObject.Parse(i.ToString());
                LogModel log = new LogModel(item);
                logs!.Add(log);
            }
        }
    }

    public simpleData createSimple()
    {
        simpleData data = new simpleData();
        data.Id = _id;
        data.name = name;
        data.region = region;
        data.location = location;
        data.passcode = passcode;

        return data;
    }

    public struct SyncResult<T>
    {
        public List<T> Added;
        public List<T> Removed;
    }

    public static SyncResult<T> SyncLists<T>(List<T> source, List<T> target)
        where T : struct
    {
        var sourceSet = new HashSet<T>(source);
        var targetSet = new HashSet<T>(target);

        var added = new List<T>();
        var removed = new List<T>();

        //Detect removed
        foreach (var item in target)
        {
            if (!sourceSet.Contains(item))
                removed.Add(item);
        }

        //Detect added
        foreach (var item in source)
        {
            if (!targetSet.Contains(item))
                added.Add(item);
        }

        //Apply removals
        target.RemoveAll(item => sourceSet.Contains(item) == false);

        //Apply additions
        target.AddRange(added);

        return new SyncResult<T>
        {
            Added = added,
            Removed = removed
        };
    }

}