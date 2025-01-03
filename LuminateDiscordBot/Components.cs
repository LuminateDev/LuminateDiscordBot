using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using LuminateDiscordBot.Objects;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Runtime.InteropServices;

namespace LuminateDiscordBot
{
    public class Components : InteractionModuleBase
    {

        private readonly DataContext _dataContext;
        private readonly Utils _utils;
        public Components(DataContext context, Utils utils)
        {
            this._dataContext = context;
            this._utils = utils;
        }

        [ComponentInteraction("ticket-start")]
        public async Task HandleTicketMenu()
        {
            IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

            TicketCategory? ticket = DBManager.GetTicketCategoryFromName(interaction.Data.Values.First());
            if (ticket?.TicketDataAutoResponse != null)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Attention.";
                embed.Description = ticket.TicketDataAutoResponse;
                embed.Color = Color.Blue;
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Constants.FOOTER_TEXT
                };

                ComponentBuilder components = new ComponentBuilder();
                components.WithButton("Open a ticket anyways", $"ticket-force:{ticket.TicketDataName}", ButtonStyle.Success);
                components.WithButton("Dismiss", "ticket-dismiss", ButtonStyle.Danger);

                await RespondAsync("", new[] { embed.Build() }, ephemeral: true, components: components.Build());
                return;
            }

            await RespondWithModalAsync<Models.TicketCreationModalModel>($"ticket-modal:{ticket?.TicketDataName}");


        }

        [ComponentInteraction("ticket-dismiss")]
        public async Task HandleTicketDismiss()
        {
            IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;
            await interaction.DeferAsync();
            await interaction.DeleteOriginalResponseAsync();
        }

        [ComponentInteraction("ticket-force:*")]
        public async Task HandleTicketForce(string dataName)
        {
            await RespondWithModalAsync<Models.TicketCreationModalModel>($"ticket-modal:{dataName}");
        }

        [ModalInteraction("ticket-modal:*")]
        public async Task HandleModal(string dataName, Models.TicketCreationModalModel modal)
        {
            TicketCategory? ticket = DBManager.GetTicketCategoryFromName(dataName);
            ITextChannel channel = await _utils.CreateTicketChannel(this);

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Ticket created!";
            embed.Description = $"Your ticket has been created successfully!\nCheck <#{channel.Id}> to discuss your issue with Luminate Staff.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
            };

            ComponentBuilder components = new ComponentBuilder();
            components.WithButton("Close this ticket", $"ticket-close:{channel.Id}", ButtonStyle.Danger);


            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            await channel.SendMessageAsync($"<@&{_utils.RoleConfig[Constants.TICKET_ROLE_IDENTIFIER]}>", false, Responses.TicketInitMessageEmbed(ticket!.TicketTopic, modal.Reason, Context.Interaction.User.Id), components: components.Build());

        }

        [ComponentInteraction("ticket-close:*")]
        public async Task CloseTicket(string channelId)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            bool hasRole = user.Roles.FirstOrDefault(x => x.Id == _utils.RoleConfig[Constants.TICKET_ROLE_IDENTIFIER]) != null;

            if (!hasRole)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Color = Color.Blue;
                embed.Title = "Access Denied!";
                embed.Description = "You can not manually close a ticket, Team Luminate will handle this for you once your request is completed!";
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Constants.FOOTER_TEXT
                };

                await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
                return;
            }

            ITextChannel channel = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(channelId));
            await RespondAsync("Closing...");
            await channel.DeleteAsync();

        }


        [ModalInteraction("ticket-category-modification:*")]
        public async Task ModifyTicketCategoryModal(string categoryId, Models.TicketManagementModalModel modal)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Timestamp = DateTime.Now;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Constants.FOOTER_TEXT
            };

            if (categoryId == Constants.TICKET_CATEGORY_AUTOCOMPLETE_ADD_KEY)
            {

                Models.Database.TicketCategory newCategory = new Models.Database.TicketCategory()
                {
                    TicketTopic = modal.TopicName,
                    TicketDataName = modal.TopicName,
                    CategoryAliases = modal.TopicKeywords,
                    AutoResponseEnabled = !string.IsNullOrEmpty(modal.TopicAutoResponse),
                    TicketDataAutoResponse = modal.TopicAutoResponse,
                    TicketDataDescription = modal.TopicDescription,
                };
                
                embed.Title = "Ticket Category added!";
                embed.Description = $"Successfully added **{modal.TopicName}** ({newCategory.CategoryId}) as a Ticket Category.";
                await RespondAsync("", new[] { embed.Build() }, ephemeral:true);
                await _dataContext.SaveChangesAsync();
                return;
            }

            var targetCategory = _dataContext.TicketCategories.FirstOrDefault(entry => entry.CategoryId == categoryId);
            if (targetCategory == null)
            {
                await RespondAsync("", new[] { Responses.InvalidActionEmbed() }, ephemeral: true);
                return;
            }

            targetCategory.TicketTopic = modal.TopicName;
            targetCategory.TicketDataName = modal.TopicName;
            targetCategory.CategoryAliases = modal.TopicKeywords;
            targetCategory.AutoResponseEnabled = !string.IsNullOrEmpty(modal.TopicAutoResponse);
            targetCategory.TicketDataAutoResponse = modal.TopicAutoResponse;
            targetCategory.TicketDataDescription = modal.TopicDescription;

            embed.Title = "Ticket Category modified!";
            embed.Description = $"Successfully modified **{modal.TopicName}** ({targetCategory.CategoryId}).";
            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            await _dataContext.SaveChangesAsync();
        }

    }
}
