using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;
using System.Reflection;

namespace LuminateDiscordBot
{
    internal class Program
    {
        public static ServiceProvider? _services = null;
        public static InteractionService? _interactionService;
        public static DiscordSocketClient? client;

        private static Utils _utils = new Utils();

        static async Task Main(string[] args)
        {
            
            Console.WriteLine("Starting Bot...");
            await _utils.FileCheck();
            var config = _utils.GetConfig();
            
            if (config.BotToken == "") { Console.WriteLine("Please setup the config."); Console.ReadKey(); Environment.Exit(0); }

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                GatewayIntents = Discord.GatewayIntents.All,
                LogLevel = Discord.LogSeverity.Debug,
                UseInteractionSnowflakeDate = false,
            };
            client = new DiscordSocketClient(socketConfig);

            await client.LoginAsync(TokenType.Bot, config.BotToken);
            await client.StartAsync();
            _interactionService = new InteractionService(client.Rest);
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Events

            client.Ready += async () =>
            {
                try
                {
                    await _interactionService.RegisterCommandsGloballyAsync(true);
                    await client.SetGameAsync("/luminate-help", "", ActivityType.Listening);
                    Console.WriteLine("Bot Online!");
                }catch(Exception e) { Console.WriteLine($"Unable to finalize initialization: {e.Message}"); }
            };
            client.InteractionCreated += async (socketInteraction) =>
            {
                SocketInteractionContext context = new SocketInteractionContext(client, socketInteraction);
                var result = await _interactionService.ExecuteCommandAsync(context, _services);
            };
            client.UserJoined += async (guildUser) =>
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Welcome to Luminate";
                embed.Color = Color.Blue;
                embed.Description = $"Welcome **{guildUser.Mention}** to the Luminate Discord Server!\n" +
                    $"We hope you have a nice stay.";
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Constants.FOOTER_TEXT
                };
                try
                {
                    await guildUser.Guild.GetTextChannel(_utils.ChannelConfig[Constants.WELCOME_CHANNEL_IDENTIFIER]).SendMessageAsync("", false, embed.Build());
                }
                catch (Exception e) { await Console.Out.WriteLineAsync(e.Message); }

            };

#if DEBUG
            // Only prints out Log Messages if the bot is run in a debug build, since log messages are useless in prod
            client.Log += (logMessage) =>
            {
                Console.WriteLine(logMessage.Message);
                return Task.CompletedTask;
            };
#endif

            // Events end here


            Console.WriteLine("Creating Database...");
            using (var scope = _services!.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>())
                {
                    dbContext.Database.EnsureCreated();

                    await _utils.ReloadChannelConfig(dbContext.DataConfigs.Where(entry => entry.DataType == Models.Database.DataConfig.DataTypes.CHANNEL).ToList());
                    await _utils.ReloadRoleConfig(dbContext.DataConfigs.Where(entry => entry.DataType == Models.Database.DataConfig.DataTypes.ROLE).ToList());
                }
            }
            Console.WriteLine("Database Created.");

            Console.WriteLine("Setup Complete");

            await Task.Delay(-1);
        }

        static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite($"Data Source={Constants.APP_ROOT}/database.db");
            });
            services.AddSingleton<Utils>(_utils);

            return services.BuildServiceProvider();
        }

    }
}
