namespace FoxholeToolsAPI.DiscordApi.Models.Dtos;

public class DiscordApiGuildMemberDto
{
    public string avatar { get; set; }
    public object communication_disabled_until { get; set; }
    public int flags { get; set; }
    public bool is_pending { get; set; }
    public string joined_at { get; set; }
    public string nick { get; set; }
    public bool pending { get; set; }
    public object premium_since { get; set; }
    public string[] roles { get; set; }
    public DiscordApiUserDto DiscordApiUserDto { get; set; }
    public bool mute { get; set; }
    public bool deaf { get; set; }
}

