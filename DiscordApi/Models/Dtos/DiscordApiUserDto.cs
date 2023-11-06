namespace FoxholeToolsAPI.DiscordApi.Models.Dtos;

public class DiscordApiUserDto
{
    public string id { get; set; }
    public string username { get; set; }
    public object global_name { get; set; }
    public object display_name { get; set; }
    public string avatar { get; set; }
    public string discriminator { get; set; }
    public int public_flags { get; set; }
    public object avatar_decoration { get; set; }
}

