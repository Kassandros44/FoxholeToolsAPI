
using Discord;
using Discord.WebSocket;

namespace FoxholeToolsAPI.DiscordApi
{
    public class DiscordBotService
    {

        private readonly DiscordSocketClient _client;

        public DiscordBotService() {

            _client = new DiscordSocketClient(new DiscordSocketConfig { 
            
                GatewayIntents =
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.MessageContent
            
            });
        
        }

    }
}
