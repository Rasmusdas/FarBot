using DadBotNet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DadBotNet
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                string token = services.GetRequiredService<ConfigService>().GetField("token");

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<CommandService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<ConfigService>()
                .BuildServiceProvider();
        }
    }
}