using Newtonsoft.Json.Linq;
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using FoxholeToolsAPI.DiscordApi.Models;

public class UserModel {

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id;
    public string discordId = string.Empty;
    public string username = string.Empty;
    public string Passkey = string.Empty;
    public string globalName = string.Empty;
    public string[] roles;
    public string rank = string.Empty;

    public UserModel() { }

    public UserModel(JObject jobject){
        
        Id = string.Empty;

        if (jobject.ContainsKey("id"))
        {
            discordId = (string)jobject["id"]!;
        }
        if (jobject.ContainsKey("username")){
            username = (string)jobject["username"]!;
        }
        if (jobject.ContainsKey("global_name"))
        {
            globalName = (string)jobject["global_name"]!;
        }

    }

    public UserModel(string id, JObject jobject){
        
        Id = id;

        if(jobject.ContainsKey("username")){
            username = (string)jobject["username"]!;
        }

        if (jobject.ContainsKey("global_name"))
        {
            globalName = (string)jobject["global_name"]!;
        }

    }

    #region DISCORD

    public UserModel(DiscordApiUser discordApiUser, DiscordApiGuildMemberDto guildMemberData)
    {
        Id = string.Empty;
        discordId = discordApiUser.Id;
        username = discordApiUser.Username;
        roles = guildMemberData.roles;
    }

    public void SetRolesList(DiscordApiGuildMemberDto dto)
    {
        foreach (var item in dto.roles)
        {
            switch (item)
            {
                case "311665931602362369":
                    rank = "Jr Commissar";
                    break;
            }
        }
    }

    /*switch (item)
            {
                case "400885800390230027":
                    rank = "High Command";
                    break;
                case "303835804478865408":
                    rank = "Commissar";
                    break;
                case "311665931602362369":
                    rank = "Jr Commissar";
                    break;
                case "1037019825823817758":
                    rank = "Senior Watchmaster";
                    break;
                case "282727563430985728":
                    rank = "Watchmaster";
                    break;
                case "216336777525460992":
                    rank = "Junior Watchmaster";
                    break;
                case "216335909589614593":
                    rank = "Senior Kriegsman";
                    break;
                case "282726022720258050":
                    rank = "Kriegsman";
                    break;
                case "1155569054384406659":
                    rank = "Grenadier";
                    break;
                case "582240086293217322":
                    rank = "Battlefield Quatermaster";
                    break;
                case "347325953380450316":
                    rank = "Staff Sergeant";
                    break;
                case "346515430703104000":
                    rank = "Sergeant";
                    break;
                case "345584858602668043":
                    rank = "Corporal";
                    break;
                case "465564735774130196":
                    rank = "Lance Corporal";
                    break;
                case "465565450038673429":
                    rank = "Private First Class";
                    break;
                case "216334759549337600":
                    rank = "Private";
                    break;
                case "282709638003163146":
                    rank = "Conscript (RIP)";
                    break;
            }*/

    #endregion

}