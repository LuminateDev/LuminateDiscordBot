using Discord;
using Discord.Interactions;
using Discord.Rest;
using LuminateDiscordBot.Objects;

namespace LuminateDiscordBot
{
    public class Components : InteractionModuleBase
    {

        [ComponentInteraction("ticket-start")]
        public async Task HandleTicketMenu()
        {
            IComponentInteraction interaction = (IComponentInteraction)Context.Interaction;

            TicketCategory ticket = DBManager.GetTicketCategoryFromName(interaction.Data.Values.First());
            if(ticket.TicketDataAutoResponse != null)
            {
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Attention.";
                embed.Description = ticket.TicketDataAutoResponse;
                embed.Color = Color.Blue;
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = "Luminate - Your ideas shine bright"
                };

                ComponentBuilder components = new ComponentBuilder();
                components.WithButton("Open a ticket anyways", "ticekt-force", ButtonStyle.Success);
                components.WithButton("Dismiss", "ticket-dismiss", ButtonStyle.Danger);

                await RespondAsync("", new[] { embed.Build() }, ephemeral:true);
                return;
            }

            ITextChannel channel = await Utils.CreateTicketChannel(this);
            
            
        }

        [ComponentInteraction("ticket-dismiss")]
        public async Task HandleTicketDismiss()
        {
            await DeleteOriginalResponseAsync();
        }

        [ComponentInteraction("ticket-force")]
        public async Task HandleTicketForce()
        {

        }
    }
}
