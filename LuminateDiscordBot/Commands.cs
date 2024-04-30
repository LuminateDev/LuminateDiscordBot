using Discord;
using Discord.Interactions;

namespace LuminateDiscordBot
{
    public class Commands : InteractionModuleBase
    {
        [SlashCommand("mod-setchannelrule", "Adds or updates the channel config")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task ModifyChannelRules([Summary("channel_identifier", "The internal Identifier you modify")] string channelIdentifier, [ChannelTypes(Discord.ChannelType.Text, Discord.ChannelType.Voice)] IChannel targetChannel)
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

        [SlashCommand("mod-echoattachment", "Repeats a message from file content")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task EchoMessageFromFile(IAttachment file, bool asEmbed = false, string embedTitle = null)
        {

            string message = new HttpClient().GetAsync(file.Url).Result.Content.ReadAsStringAsync().Result;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Echoing...";
            embed.Description = $"Your message will be echoed to the current channel.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - {Utils.DefaultSloganText}"
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);

            if (asEmbed)
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
            }
            else
            {
                await Context.Channel.SendMessageAsync(message);
                return;
            }
        }

        [SlashCommand("mod-setprivatevcchannel", "Sets the channel for users to create a private vc")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task SetPrivateVoiceChannelChannel([ChannelTypes(Discord.ChannelType.Category)] ICategoryChannel category ,[ChannelTypes(Discord.ChannelType.Voice)] IChannel targetVoiceChannel, [ChannelTypes(Discord.ChannelType.Text)] IChannel targetInfoChannel)
        {
            ulong emojiId = 1227219262230233088;
            var changeName = Emote.Parse("<:edit:1227869048931352606>");
            var changeLimit = Emote.Parse("<:limit:1227869061933699153>");
            var chanePrivacy = Emote.Parse("<:privacy:1227869072599945277>");
            var trustPerson = Emote.Parse("<:addperson:1227869084671148082>");
            var untrustPerson = Emote.Parse("<:removeperson:1227869098675929203>");
            var transferOwnership = Emote.Parse("<:ownership:1227869113385226240>");
            var deleteChannel = Emote.Parse("<:delete:1227869124181626892>");
            ulong guild_id = Context.Interaction.GuildId.Value;
            ITextChannel infoChannel = await Context.Guild.GetTextChannelAsync(targetInfoChannel.Id);
            DBManager.ModifyVoiceChannelConfig(category.Id ,targetVoiceChannel.Id, guild_id);
            var changeChannelNameButton = new ComponentBuilder().WithButton(" ", "changeName", ButtonStyle.Secondary, changeName)
                .WithButton(" ", "changeLimit", ButtonStyle.Secondary, changeLimit)
                .WithButton(" ", "changePrivacy", ButtonStyle.Secondary, chanePrivacy)
                .WithButton(" ", "trustPerson", ButtonStyle.Secondary, trustPerson)
                .WithButton(" ", "untrustPerson", ButtonStyle.Secondary, untrustPerson)
                .WithButton(" ", "transferOwnership", ButtonStyle.Secondary, transferOwnership)
                .WithButton(" ", "deleteChannel", ButtonStyle.Secondary, deleteChannel);
            
            
            EmbedBuilder tempChannelInterface = new EmbedBuilder();
            tempChannelInterface.Color = Color.Blue;
            tempChannelInterface.Title = "Temp Voice Interface";
            tempChannelInterface.Description = "With this interface you're able to control your private voice channel. \n You can also use **/tmp** to control your channel.";
            tempChannelInterface.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - {Utils.DefaultSloganText}"
            };
            await infoChannel.SendMessageAsync("", false, tempChannelInterface.Build(), components: changeChannelNameButton.Build());
            await RespondAsync("Successfully set the tempvoice channel", ephemeral: true);
        }
    }
}
