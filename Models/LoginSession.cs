using FoxholeToolsAPI.DiscordApi.Models;
using System.Collections.Concurrent;

namespace FoxholeToolsAPI.Models
{
    public class LoginSession
    {

        public bool Completed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public UserModel? User { get; set; }
        public string? AccessToken { get; set; }

    }

}
