using System.Text.Json.Serialization;

namespace FoxholeToolsAPI.DiscordApi.Models.Dtos;

public class AccessTokenResponse
{
    [JsonPropertyName("Access_Token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("Expires_In")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("Refresh_Token")]
    public string RefreshToken { get; set; } = string.Empty;
    [JsonPropertyName("Scope")]
    public string Scope { get; set; } = string.Empty;
    [JsonPropertyName("Token_Type")]
    public string TokenType { get; set; } = string.Empty;
}
