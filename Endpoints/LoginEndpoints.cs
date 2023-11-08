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
        string discordClientSecret = "VeV8QtftgcTiFhwT7H_rQb0Tfh9Ps2FP";
        string redirectUri = "https://foxholetools.azurewebsites.net/discord-login";

        //Redirect for Auth
        app.Map("/discord-login", () => {
            return "Login!!";
        });

        //Code exchange for token and user data
        app.MapGet("/discord-login/{code}", async (string code) => {

            /*
             * TODO: 
             * Spilt this out into seperate methods for the two logic sets
             * Add the logic for user profile creation (automatic at 1st login)
             */

            Console.WriteLine(code);
            string ACCESSTOKEN;
            string APIRESPONSE;
            ulong guildId = 213905419108614145;

            DiscordApiClient client = new DiscordApiClient(new HttpClient());

            ACCESSTOKEN = GetToken(code, discordClientId, discordClientSecret, redirectUri);
            Console.WriteLine(ACCESSTOKEN);

            DiscordApiUser user = await client.GetUsersMe(ACCESSTOKEN);

            GetUserGuildMemberResponse info = await client.GetUserGuildMember(ACCESSTOKEN, guildId);
            Console.WriteLine(info.DiscordApiGuildMemberDto.ToJson());

            if (info.DiscordApiGuildMemberDto.roles.Contains("333847333643223052"))
            {
                
                return Results.Json(CreateUserEntry(info, user));
            }
            else
            {
                Console.WriteLine("User Login Failed - NonDiscordMember");
                return Results.Problem("User Login Failed - NonDiscordMember");
            }
                
        });
    }

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

    private static UserModel CreateUserEntry(
        GetUserGuildMemberResponse response,
        DiscordApiUser discordApiUser
        )
    {
        UserModel userModel = new UserModel(response.DiscordApiGuildMemberDto, discordApiUser);
        string id = userModel.discordId;
        var userCollection = DBUtils.ConnectToMongo<UserModel>("Users");
        var builder = Builders<UserModel>.Filter;
        var filter = builder.Eq(u => u.discordId, id);
        var check = userCollection.Find<UserModel>(filter).FirstOrDefault();
        if (check != null)
        {
            Console.WriteLine("user already in list");
            return check;
        }
        else
        {
            userCollection.InsertOne(userModel);
            return userModel;
        }
    }

}
