using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Events
{
    internal class UserJoinHandler
    {
        public static async Task HandleUserServerJoin(SocketGuildUser user)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Welcome to Luminate";
            embed.Color = Color.Blue;
            embed.Description = $"Welcome **<@{user.Id}>** to the Luminate Discord Server!\n" +
                $"We hope you have a nice stay.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - Your ideas shind bright"
            };
            try
            {
                await user.Guild.GetTextChannel(Utils.ChannelConfig["welcome_channel"]).SendMessageAsync("", false, embed.Build());
            }catch(Exception e) { await Console.Out.WriteLineAsync(e.Message); }
            
        }
    }
}
