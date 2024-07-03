using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using LuminateDiscordBot.Objects;

namespace LuminateDiscordBot
{
    public class Components : InteractionModuleBase
    {

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
                    Text = Utils.SloganText
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
            ITextChannel channel = await Utils.CreateTicketChannel(this);

            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "Ticket created!";
            embed.Description = $"Your ticket has been created successfully!\nCheck <#{channel.Id}> to discuss your issue with Luminate Staff.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Utils.SloganText
            };

            ComponentBuilder components = new ComponentBuilder();
            components.WithButton("Close this ticket", $"ticket-close:{channel.Id}", ButtonStyle.Danger);


            await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
            await channel.SendMessageAsync($"<@&{Utils.RoleConfig["ticket_role"]}>", false, Responses.TicketInitMessage(ticket!.TicketTopic, modal.Reason, Context.Interaction.User.Id), components: components.Build());

        }

        [ComponentInteraction("ticket-close:*")]
        public async Task CloseTicket(string channelId)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            bool hasRole = user.Roles.FirstOrDefault(x => x.Id == Utils.RoleConfig["ticket_role"]) != null;

            if (!hasRole)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Color = Color.Blue;
                embed.Title = "Access Denied!";
                embed.Description = "You can not manually close a ticket, Team Luminate will handle this for you once your request is completed!";
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Utils.SloganText
                };

                await RespondAsync("", new[] { embed.Build() }, ephemeral: true);
                return;
            }

            ITextChannel channel = await Context.Guild.GetTextChannelAsync(Convert.ToUInt64(channelId));
            await RespondAsync("Closing...");
            await channel.DeleteAsync();

        }

    }
}
