using MongoDB.Driver;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FoxholeToolsAPI.DiscordApi;
using FoxholeToolsAPI.DiscordApi.Models;
using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using MongoDB.Bson;
using System.Reflection.Metadata.Ecma335;

public class LoginEndpoints
{

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
            return "Login!!";
        });

        //Code exchange for token and user data
        app.MapGet("/discord-login/{code}", async (string code) => {

            /*
             * TODO: 
             * Clean up
             */

            Console.WriteLine(code);
            string ACCESSTOKEN;
            string APIRESPONSE;
            ulong guildId = 407499436617629718;

            DiscordApiClient client = new DiscordApiClient(new HttpClient(), discordConfig);

            Console.WriteLine($"reUri: {discordConfig.RedirectUrl}");
            //ACCESSTOKEN = GetToken(code, discordClientId, discordClientSecret, discordConfig.RedirectUrl);
            AccessTokenResponse accessTokenResponse = await client.GetOauth2Token(code);
            Console.WriteLine(accessTokenResponse.AccessToken);

            DiscordApiUser user = await client.GetUsersMe(accessTokenResponse.AccessToken);
            ulong userId = ulong.Parse(user.Id);
            Console.WriteLine(discordConfig.Token);
            Console.WriteLine(guildId);
            Console.WriteLine(userId);
            GetUserGuildMemberResponse guildData = await client.GetUserGuildData(discordConfig.Token, guildId, userId);
            Console.WriteLine(user.Username);
            Console.WriteLine(user.Id);
            Console.WriteLine(guildData.DiscordApiGuildMemberDto.roles.ToJson());

            Console.WriteLine(user.ToJson());

            if (user.Id != null)
            {
                
                return Results.Json(CreateUserEntry(user, guildData.DiscordApiGuildMemberDto));
            }
            else
            {
                Console.WriteLine("User Login Failed - NonDiscordMember");
                return Results.Problem("User Login Failed - NonDiscordMember");
            }


                
        });

        #endregion

    }

    #region DISCORD LOGIC

    private static string GetToken(string code, string discordClientId, string discordClientSecret, string redirectUri) 
    {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/oauth2/token");
        webRequest.Method = "POST";
        string parameters = "client_id=" + discordClientId + "&client_secret=" + discordClientSecret + "&grant_type=authorization_code&code=" + code + "&redirect_uri=" + redirectUri + "";
        byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = byteArray.Length;
        Stream postStream = webRequest.GetRequestStream();

        postStream.Write(byteArray, 0, byteArray.Length);
        postStream.Close();
        WebResponse response = webRequest.GetResponse();
        postStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(postStream);
        string responseFromServer = reader.ReadToEnd();

        Console.Write(responseFromServer);

        string tokenInfo = responseFromServer.Split(',')[1].Split(":")[1];
        string access_token = tokenInfo.Trim().Substring(1, tokenInfo.Length - 3);
        return access_token;
    }

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

    #endregion

}
