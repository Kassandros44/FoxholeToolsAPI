namespace FoxholeToolsAPI.DiscordApi.Models;

public class DiscordApiUser
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Discriminator { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
}
