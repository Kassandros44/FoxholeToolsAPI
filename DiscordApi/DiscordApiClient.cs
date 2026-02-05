using System.Net.Http.Headers;
using System.Net;
using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using FoxholeToolsAPI.DiscordApi.Models;
using System.Text;

namespace FoxholeToolsAPI.DiscordApi;

public class DiscordApiClient : IDiscordApiClient
{
    private readonly HttpClient _httpClient;
    private readonly DiscordApiConfiguration _discordApiConfiguration;

    public DiscordApiClient(
        HttpClient httpClient,
        DiscordApiConfiguration discordApiConfiguration)
    {
        _httpClient = httpClient;
        _discordApiConfiguration = discordApiConfiguration;
    }

    public async Task<AccessTokenResponse?> GetOauth2Token(string code)
    {
        var content = GetBaseContent();

        content.Add(new KeyValuePair<string, string>("redirect_uri", _discordApiConfiguration.RedirectUrl));
        content.Add(new KeyValuePair<string, string>("code", code));
        content.Add(new KeyValuePair<string, string>("grant_type", "authorization_code"));

        using var formUrlEncodedContent = new FormUrlEncodedContent(content);

        formUrlEncodedContent.Headers.Clear();
        formUrlEncodedContent.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(new Uri("https://discord.com/api/v10/oauth2/token"), formUrlEncodedContent);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
    }

    public async Task<DiscordApiUser?> GetUsersMe(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync("https://discord.com/api/users/@me");

        var b = (MemoryStream)await response.Content.ReadAsStreamAsync();
        var s = Encoding.ASCII.GetString(b.ToArray());

        try
        {
            var discordInformation = await response.Content.ReadFromJsonAsync<DiscordApiUser>();
            return discordInformation;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public async Task<GetUserGuildMemberResponse?> GetUserGuildData(
        string botToken,
        ulong guildId,
        ulong memberId)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", botToken);
        var response = await _httpClient.GetAsync($"https://discord.com/api/v10/guilds/{guildId}/members/{memberId}");
        DiscordApiGuildMemberDto? discordInformation = null;

        Console.WriteLine(response.StatusCode.ToString());

        //var remaining = Convert.ToInt32(response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault());
        //var reset = DateTime.UnixEpoch.AddSeconds(Convert.ToDouble(response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault()));

        if(response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                discordInformation = await response.Content.ReadFromJsonAsync<DiscordApiGuildMemberDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        else
        {
            Console.WriteLine(response.StatusCode.ToString());
        }

        Console.WriteLine(discordInformation);
        return new GetUserGuildMemberResponse(discordInformation, 1, DateTime.Now);

    }

    private List<KeyValuePair<string, string>> GetBaseContent()
    {
        return new List<KeyValuePair<string, string>>
        {
            new("client_secret", _discordApiConfiguration.ClientSecret),
            new("client_id", _discordApiConfiguration.ClientId)
        };
    }
}

