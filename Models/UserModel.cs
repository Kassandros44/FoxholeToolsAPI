using Newtonsoft.Json.Linq;
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class UserModel {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id;
    public string username = string.Empty;
    public string Passkey = string.Empty;

    private enum RANK{
        RIP,
        Member,
        KR,
        Officer,
        HighCommand
    }

    public UserModel(JObject jobject){
        
        Id = string.Empty;

        if(jobject.ContainsKey("username")){
            username = (string)jobject["username"]!;
        }

    }

    public UserModel(string id, JObject jobject){
        
        Id = id;

        if(jobject.ContainsKey("username")){
            username = (string)jobject["username"]!;
        }

    }

}