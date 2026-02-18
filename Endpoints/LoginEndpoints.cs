using MongoDB.Driver;
using System.Net;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using FoxholeToolsAPI.DiscordApi;
using FoxholeToolsAPI.DiscordApi.Models;
using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using MongoDB.Bson;
using System.Reflection.Metadata.Ecma335;
using FoxholeToolsAPI.Models;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;

public class LoginEndpoints
{

    static ConcurrentDictionary<string, LoginSession> LoginSessions = new();

    public static void Map(WebApplication app)
    {

        string discordClientId = "1138105236007948341";
        string discordClientSecret = "ivn7t4Xc1sqS-zMoB2Gsbe3F5NBau0ht";
        string redirectUri = "https://foxholetools.azurewebsites.net/discord-login";
        var discordConfig = new DiscordApiConfiguration();
        app.Configuration.GetSection(DiscordApiConfiguration.SettingsName).Bind(discordConfig);

        #region DISCORD MAPS

        //Redirect for Auth
        app.Map("/discord-login", () => {
            string clientId = discordConfig.ClientId;
            string redirectUri = discordConfig.RedirectUrl;
            string scope = "identify";

            string sessionId = Guid.NewGuid().ToString("N");

            LoginSessions[sessionId] = new LoginSession();

            string url = 
                "https://discord.com/api/oauth2/authorize" +
                $"?client_id={clientId}" +
                $"&redirect_uri={Uri.UnescapeDataString(redirectUri)}" +
                $"&response_type=code" +
                $"&scope={Uri.EscapeDataString(scope)}" +
                $"&state={sessionId}";

            return Results.Ok(new
            {
                sessionId,
                url
            });
        });

        //Redirect callback for unity
        app.MapGet("/discord-login/callback",
            async (
                string code,
                string state
            ) =>
        {

            if (!LoginSessions.TryGetValue(state, out var session))
                return Results.BadRequest("Invalid session");

            var discord = new DiscordApiClient(new HttpClient(), discordConfig);
            var token = await discord.GetOauth2Token(code);
            var user = await discord.GetUsersMe(token!.AccessToken);
            var guildData = await discord.GetUserGuildData(discordConfig.Token, 407499436617629718, ulong.Parse(user!.Id));
            session.User = CreateUserEntry(user!, guildData!.DiscordApiGuildMemberDto!);

            session.AccessToken = token.AccessToken;
            session.Completed = true;

            return Results.Content("""
                Foxhole Tools login successful.
                You can close this window and return to the app.
                """, "text/plain");
        });

        app.MapGet("/discord-login/status", (string sessionId) =>
        {

            Console.WriteLine("status check called");

            if(!LoginSessions.TryGetValue(sessionId, out var session))
                return Results.NotFound();

            if(!session.Completed)
                return Results.Ok(new {completed = false});

            Console.WriteLine(session.User.ToJson());

            return Results.Ok(new
            {
                completed = true,
                user = session.User,
                token = session.AccessToken
            });

        });

        #endregion

    }

    #region DISCORD LOGIC

    private static UserModel CreateUserEntry(DiscordApiUser discordApiUser, DiscordApiGuildMemberDto guildMemberData)
    {
        UserModel userModel = new UserModel(discordApiUser, guildMemberData);
        string id = userModel.discordId;
        var userCollection = DBUtils.ConnectToMongo<UserModel>("Users");
        var builder = Builders<UserModel>.Filter;
        var filter = builder.Eq(u => u.discordId, id);
        var check = userCollection.Find<UserModel>(filter).FirstOrDefault();
        if (check != null)
        {
            Console.WriteLine("user already in list");
            userCollection.DeleteOne(filter);
            userCollection.InsertOne(userModel);
            return userModel;
        }
        else
        {
            userCollection.InsertOne(userModel);
            return userModel;
        }
    }

    private static UserModel CreateUserEntry(DiscordApiUser discordApiUser)
    {
        UserModel userModel = new UserModel(discordApiUser);
        string id = userModel.discordId;
        var userCollection = DBUtils.ConnectToMongo<UserModel>("Users");
        var builder = Builders<UserModel>.Filter;
        var filter = builder.Eq(u => u.discordId, id);
        var check = userCollection.Find(filter).FirstOrDefault();
        if (check != null)
        {
            Console.WriteLine("user already in list");
            userCollection.DeleteOne(filter);
            userCollection.InsertOne(userModel);
            return userModel;
        }
        else
        {
            Console.WriteLine($"User created: {userModel.username}");
            userCollection.InsertOne(userModel);
            return userModel;
        }
    }

    #endregion

}
