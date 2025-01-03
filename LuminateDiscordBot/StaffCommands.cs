using Discord;
using Discord.Interactions;

namespace LuminateDiscordBot
{
    [Group("staff", "Contains all staff commands")]
    [RequireUserPermission(Discord.GuildPermission.BanMembers)]
    [DefaultMemberPermissions(GuildPermission.BanMembers)]
    [CommandContextType(Discord.InteractionContextType.Guild)]
    public class StaffCommands : InteractionModuleBase
    {
        private readonly DataContext _dataContext;
        private readonly Utils _utils;
        public StaffCommands(DataContext dataContext, Utils utils)
        {
            this._dataContext = dataContext;
            this._utils = utils;
        }

        [SlashCommand("echo", "Repeats a message as the bot")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task EchoMessage(string message, bool asEmbed = false, string? embedTitle = null)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Echoing...";
            embed.Description = $"Your message will be echoed to the current channel.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
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
                    Text = Constants.FOOTER_TEXT
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


        [SlashCommand("init-tickets", "Initializes the ticket Select Menu")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task InitTicketMessage()
        {
            List<Models.Database.TicketCategory> tickets = _dataContext.TicketCategories.Take(25).ToList();

            SelectMenuBuilder menu = new SelectMenuBuilder();

            foreach (var ticket in tickets)
            {
                menu.AddOption(ticket.TicketTopic, ticket.CategoryId, ticket.TicketDataDescription);
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
                Text = Constants.FOOTER_TEXT
            };

            ComponentBuilder components = new ComponentBuilder();
            components.WithSelectMenu(menu);

            await RespondAsync("Initializing...", ephemeral: true);
            await Context.Channel.SendMessageAsync("", false, embed.Build(), components: components.Build());


        }


        [SlashCommand("set-channel-rule", "Adds or updates the channel config")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ModifyChannelRules([Summary("channel_identifier", "The internal Identifier you modify")] string channelIdentifier, [ChannelTypes(Discord.ChannelType.Text, Discord.ChannelType.Voice, Discord.ChannelType.Category)] IChannel targetChannel)
        {
            var configEntry = _dataContext.DataConfigs.FirstOrDefault(entry => entry.DataType == Models.Database.DataConfig.DataTypes.CHANNEL && entry.DataName == channelIdentifier);
            if (configEntry != null)
            {
                configEntry.DataValue = targetChannel.Id;
            } else
            {
                var entry = new Models.Database.DataConfig()
                {
                    DataName = channelIdentifier,
                    DataType = Models.Database.DataConfig.DataTypes.CHANNEL,
                    DataValue = targetChannel.Id
                };
                _dataContext.DataConfigs.Add(entry);
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = "You have successfully updated the Channel Config.";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            await _dataContext.SaveChangesAsync();

            await _utils.ReloadChannelConfig(_dataContext.DataConfigs.Where(entry => entry.DataType == Models.Database.DataConfig.DataTypes.CHANNEL).ToList());
        }

        [SlashCommand("set-role-rule", "Adds or updates the role config")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task ModifyRoleRules([Summary("role_identifier", "The internal Identifier you modify")] string roleIdentifier, IRole targetRole)
        {
            var roleEntry = _dataContext.DataConfigs.FirstOrDefault(entry => entry.DataType == Models.Database.DataConfig.DataTypes.ROLE && entry.DataName == roleIdentifier);
            if (roleEntry != null)
            {
                roleEntry.DataValue = targetRole.Id;
            } else
            {
                var entry = new Models.Database.DataConfig()
                {
                    DataName = roleIdentifier,
                    DataType = Models.Database.DataConfig.DataTypes.ROLE,
                    DataValue = targetRole.Id
                };
                _dataContext.DataConfigs.Add(entry);
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = "You have successfully updated the Role Config.";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            await _dataContext.SaveChangesAsync();
            await _utils.ReloadRoleConfig(_dataContext.DataConfigs.Where(entry => entry.DataType == Models.Database.DataConfig.DataTypes.ROLE).ToList());
        }

        [SlashCommand("echo-attachment", "Repeats a message from file content")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task EchoMessageFromFile(IAttachment file, bool asEmbed = false, string? embedTitle = null)
        {

            string message = new HttpClient().GetAsync(file.Url).Result.Content.ReadAsStringAsync().Result;

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Echoing...";
            embed.Description = $"Your message will be echoed to the current channel.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
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
                    Text = Constants.FOOTER_TEXT
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
                }
                catch (Exception e) { Console.WriteLine(e); }

            }
        }

        [SlashCommand("modify-ticket-category", "Allows you to create or modify a ticket category")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ModifyOrCreateTicketCategory([Summary("category-id", "The Ticket category you want to modify"), Autocomplete(typeof(Autofills.TicketAutoCompleteLoader))] string ticketCategoryId)
        {
            if(ticketCategoryId == Constants.TICKET_CATEGORY_AUTOCOMPLETE_ADD_KEY)
            {
                await RespondWithModalAsync<Models.TicketManagementModalModel>($"ticket-category-modification:{Constants.TICKET_CATEGORY_AUTOCOMPLETE_ADD_KEY}");
                return;
            }
            var targetEntry = _dataContext.TicketCategories.FirstOrDefault(entry => entry.CategoryId == ticketCategoryId);
            if(targetEntry != null)
            {
                Models.TicketManagementModalModel modal = new()
                {
                    TopicAutoResponse = targetEntry.TicketDataAutoResponse,
                    TopicName = targetEntry.TicketDataName,
                    TopicKeywords = targetEntry.CategoryAliases,
                    TopicDescription = targetEntry.TicketDataDescription,
                };
                await RespondWithModalAsync<Models.TicketManagementModalModel>($"ticket-category-modification:{targetEntry.CategoryId}", modal);
                return;
            }

            await RespondAsync("", new[] { Responses.InvalidActionEmbed() }, ephemeral:true);
        }

        [SlashCommand("delete-ticket-category", "Allows you to delete a ticket category")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteTicketCategoryCommand([Summary("category-id", "The Ticket category you want to delete"), Autocomplete(typeof(Autofills.TicketAutoCompleteLoader))] string categoryId)
        {
            var targetCategory = _dataContext.TicketCategories.FirstOrDefault(entry => entry.CategoryId == categoryId);
            if(targetCategory == null)
            {
                await RespondAsync("", new[] { Responses.InvalidActionEmbed() }, ephemeral: true);
                return;
            }

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Category Removed!";
            embed.Description = $"Successfully removed Category with ID **{targetCategory.CategoryId}** from the database!";
            embed.Footer = new()
            {
                Text = Constants.FOOTER_TEXT
            };
            embed.Timestamp = DateTime.Now;
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            _dataContext.TicketCategories.Remove(targetCategory);
            await _dataContext.SaveChangesAsync();
        }

    }
}
