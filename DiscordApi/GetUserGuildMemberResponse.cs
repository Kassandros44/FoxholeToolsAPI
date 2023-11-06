using FoxholeToolsAPI.DiscordApi.Models.Dtos;

namespace FoxholeToolsAPI.DiscordApi;
public class GetUserGuildMemberResponse
{
    public readonly int CallsRemaining;
    public DiscordApiGuildMemberDto? DiscordApiGuildMemberDto;
    public DateTime ResetEpoch;

    public GetUserGuildMemberResponse(
        DiscordApiGuildMemberDto? discordApiGuildMemberDto,
        int callsRemaining,
        DateTime resetEpoch)
    {
        DiscordApiGuildMemberDto = discordApiGuildMemberDto;
        CallsRemaining = callsRemaining;
        ResetEpoch = resetEpoch;
    }
}
