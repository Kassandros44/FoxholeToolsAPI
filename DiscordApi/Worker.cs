
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.WebSocket;

namespace FoxholeToolsAPI.DiscordApi
{
    public class Worker : BackgroundService
    {

        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;

        public Worker(IConfiguration config)
        {
            _config = config;
            _client = new DiscordSocketClient();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _client.Log += msg =>
            {

                Console.WriteLine(msg);
                return Task.CompletedTask;

            };

            await _client.LoginAsync(TokenType.Bot,
                _config["DiscordSettings:Token"]);

            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}
