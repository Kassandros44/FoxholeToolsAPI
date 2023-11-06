using System.Net.Http.Headers;
using System.Net;
using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using FoxholeToolsAPI.DiscordApi.Models;
using System.Text;

namespace FoxholeToolsAPI.DiscordApi;

public class DiscordApiClient : IDiscordApiClient
{
    private readonly HttpClient _httpClient;

    public DiscordApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

    public async Task<GetUserGuildMemberResponse?> GetUserGuildMember(
        string accessToken,
        ulong guild)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync($"https://discord.com/api/v10/users/@me/guilds/{guild}/member");
        DiscordApiGuildMemberDto? discordInformation = null;

        Console.WriteLine(response.Content.ToString());

        var remaining = Convert.ToInt32(response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault());
        var reset = DateTime.UnixEpoch.AddSeconds(Convert.ToDouble(response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault()));

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

        return new GetUserGuildMemberResponse(discordInformation, remaining, reset);

    }
}

