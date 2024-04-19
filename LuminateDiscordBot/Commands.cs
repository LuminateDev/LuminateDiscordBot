using Discord;
using Discord.Interactions;

namespace LuminateDiscordBot
{
    public class Commands : InteractionModuleBase
    {
        [SlashCommand("mod-setchannelrule", "Adds or updates the channel config")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task ModifyChannelRules([Summary("channel_identifier", "The internal Identifier you modify")] string channelIdentifier, [ChannelTypes(Discord.ChannelType.Text, Discord.ChannelType.Voice, Discord.ChannelType.Category)] IChannel targetChannel)
        {
            DBManager.ModifyChannelConfig(channelIdentifier, targetChannel.Id);
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = "You have successfully updated the Channel Config.";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - Your ideas shine bright"
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
                Text = $"Luminate - Your ideas shine bright"
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
                    Text = $"Luminate - Your ideas shine bright"
                };
                await Context.Channel.SendMessageAsync("", false, echo.Build());
                return;
            } else
            {
                await Context.Channel.SendMessageAsync(message);
                return;
            }
        }

        [SlashCommand("mod-settickettopic", "Sets or adds a ticket Topic Name")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task AddOrUpdateTicketCategory([Summary("category"), Autocomplete(typeof(Autofills.TicketCategoryAutoCompleteHandler.TicketAutoCompleteLoader))] string dataname, string topic, string description)
        {
            DBManager.AddOrUpdateTicketCategory(topic, topic, description);
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = $"Ticket Category **{dataname}** has been updated!";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = "Luminate - Your ideas shine bright"
            };
        }
        

        [SlashCommand("mod-inittickets", "Initializes the ticket Select Menu")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task InitTicketMessage()
        {
            List<Objects.TicketCategory> tickets = DBManager.GetTicketCategories();

            SelectMenuBuilder menu = new SelectMenuBuilder();
            
            foreach(var ticket in tickets)
            {
                menu.AddOption(ticket.CategoryName, ticket.TicketDataName);
            }
            menu.Type = ComponentType.SelectMenu;
            menu.MaxValues = 1;
            menu.WithCustomId("ticket-start");

            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Open a Ticket";
            embed.Description = "You can use the menu below to choose a ticket category you wish to open a ticket for.";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - Your ideas shine bright"
            };

            ComponentBuilder components = new ComponentBuilder();
            components.WithSelectMenu(menu);

            await RespondAsync("Initializing...", ephemeral: true);
            await Context.Channel.SendMessageAsync("", false, embed.Build(), components: components.Build());

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
                Text = $"Luminate - Your ideas shine bright"
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
                    Text = $"Luminate - Your ideas shine bright"
                };
                await Context.Channel.SendMessageAsync("", false, echo.Build());
                return;
            }
            else
            {
                try
                {
                    await Context.Channel.SendMessageAsync(message);
                    return;
                }catch(Exception e) { Console.WriteLine(e); }

            }
        }
    }
}
