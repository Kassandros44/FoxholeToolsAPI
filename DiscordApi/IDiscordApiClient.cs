using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using FoxholeToolsAPI.DiscordApi.Models;

namespace FoxholeToolsAPI.DiscordApi;
public interface IDiscordApiClient
{
    public Task<AccessTokenResponse?> GetOauth2Token(string code);
    public Task<DiscordApiUser?> GetUsersMe(string accessToken);
    public Task<GetUserGuildMemberResponse?> GetUserGuildData(
        string botToken,
        ulong guildId,
        ulong memeberId);

    //public Task<IEnumerable<DiscordApiUserDto>?> GetUserGuilds(string accessToken);
    //refresh token
}
