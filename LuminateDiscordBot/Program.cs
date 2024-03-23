using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace LuminateDiscordBot
{
    internal class Program
    {
        public static ServiceProvider _services;
        public static InteractionService _interactionService;
        public static DiscordSocketClient client;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Bot...");
            Utils.FileCheck();
            Utils.ReadFiles();
            DBManager.InitDB();
            if(Utils.Config.BotToken == "") { Console.WriteLine("Please setup the config."); Console.ReadKey(); Environment.Exit(0); }

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                GatewayIntents = Discord.GatewayIntents.All,
                LogLevel = Discord.LogSeverity.Error,
                UseInteractionSnowflakeDate = false,
                AlwaysDownloadUsers = true,
            };
            client = new DiscordSocketClient(socketConfig);

            await client.LoginAsync(TokenType.Bot, Utils.Config.BotToken);
            await client.StartAsync();
            _interactionService = new InteractionService(client.Rest);
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);


            // Events

            client.Ready += OnReady;

            // Events end here

            await Task.Delay(-1);


            async Task OnReady()
            {
                await _interactionService.RegisterCommandsGloballyAsync(true);
                await client.SetGameAsync("/luminate-help", "", ActivityType.Listening);
                Console.WriteLine("Bot Online!");
            }
        }

    }
}
