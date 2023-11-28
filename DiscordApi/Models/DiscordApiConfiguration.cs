namespace FoxholeToolsAPI.DiscordApi.Models;

public class DiscordApiConfiguration
{

    public const string SettingsName = "DiscordSettings";
    public string LoginUrl { get; set; } = "";
    public string RedirectUrl { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string ClientId { get; set; } = "";
    public ulong ActiveWebsiteGuild { get; set; }
    public string Token { get; set; } = "";
}
