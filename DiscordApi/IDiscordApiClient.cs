using FoxholeToolsAPI.DiscordApi.Models.Dtos;
using FoxholeToolsAPI.DiscordApi.Models;

namespace FoxholeToolsAPI.DiscordApi;
public interface IDiscordApiClient
{
    //get token
    public Task<DiscordApiUser?> GetUsersMe(string accessToken);
    public Task<GetUserGuildMemberResponse?> GetUserGuildMember(
        string accessToken,
        ulong guildId);

    //public Task<IEnumerable<DiscordApiUserDto>?> GetUserGuilds(string accessToken);
    //refresh token
}
