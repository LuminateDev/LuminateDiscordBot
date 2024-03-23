using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    public class Commands : InteractionModuleBase
    {
        [SlashCommand("mod-setchannelrule", "Adds or updates the channel config")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task ModifyChannelRules([Summary("Channel Identifier", "The internal Identifier you modify")] string channelIdentifier, [ChannelTypes(Discord.ChannelType.Text, Discord.ChannelType.Voice)] IChannel targetChannel)
        {
            DBManager.ModifyChannelConfig(channelIdentifier, targetChannel.Id);
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = "You have successfully updated the Channel Config.";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - {Utils.DefaultSloganText}"
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
        }

        [SlashCommand("mod-echo", "Repeats a message as the bot")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task EchoMessage(string message, bool asEmbed = false, string embedTitle = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Echoing...";
            embed.Description = $"Your message will be echoed to the current channel.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - {Utils.DefaultSloganText}"
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);

            if(asEmbed)
            {
                EmbedBuilder echo = new EmbedBuilder();
                echo.Color = Color.Blue;
                echo.Title = embedTitle == null ? "" : embedTitle;
                echo.Description = message;
                echo.Footer = new EmbedFooterBuilder()
                {
                    Text = $"Luminate - {Utils.DefaultSloganText}"
                };
                await Context.Channel.SendMessageAsync("", false, echo.Build());
                return;
            } else
            {
                await Context.Channel.SendMessageAsync(message);
                return;
            }

        }
    }
}
