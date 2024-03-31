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
            DBManager.UpdateInternalChannelConfigs();
            if(Utils.Config.BotToken == "") { Console.WriteLine("Please setup the config."); Console.ReadKey(); Environment.Exit(0); }

            DiscordSocketConfig socketConfig = new DiscordSocketConfig()
            {
                GatewayIntents = Discord.GatewayIntents.All,
                LogLevel = Discord.LogSeverity.Info,
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
            client.InteractionCreated += Events.InteractionHandler.HandleInteraction;
            client.UserJoined += Events.UserJoinHandler.HandleUserServerJoin;
            client.UserVoiceStateUpdated += Events.UserJoinVC.HandleUserJoinVoice;
            
            //client.Log += OnLog;

            // Events end here

            Console.WriteLine("Setup Complete");

            await Task.Delay(-1);


            async Task OnReady()
            {
                try
                {
                    await _interactionService.RegisterCommandsGloballyAsync(true);
                    await client.SetGameAsync("/luminate-help", "", ActivityType.Listening);
                    Console.WriteLine("Bot Online!");
                }catch(Exception e) { Console.WriteLine(e.Message); }

            }

            async Task OnLog(LogMessage msg)
            {
                Console.WriteLine(msg.Message);
            }
        }

    }
}
