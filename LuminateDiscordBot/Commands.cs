using Discord;
using Discord.Interactions;

namespace LuminateDiscordBot
{
    public class Commands : InteractionModuleBase
    {





        [SlashCommand("mod-settickettopic", "Sets or adds a ticket Topic Name")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task AddOrUpdateTicketCategory([Summary("category"), Autocomplete(typeof(Autofills.TicketCategoryAutoCompleteHandler.TicketAutoCompleteLoader))] string dataname, string topic, string description)
        {
                DBManager.AddOrUpdateTicketCategory(topic, dataname, description);
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Updated!";
                embed.Description = $"Ticket Category **{dataname}** has been updated!";
                embed.Color = Color.Blue;
                embed.Footer = new EmbedFooterBuilder()
                {
                    Text = Utils.SloganText
                };
                await RespondAsync("", new[] { embed.Build() }, ephemeral:true);
        }

        [SlashCommand("mod-setticketresponse", "Sets a tickets auto response")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [CommandContextType(InteractionContextType.Guild)]
        public async Task SetTicketAutoResponse([Summary("category"), Autocomplete(typeof(Autofills.TicketCategoryAutoCompleteHandler.TicketAutoCompleteLoader))] string dataname, string description)
        {
            DBManager.SetTicketAutoResponse(dataname, description);
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Updated!";
            embed.Description = $"Ticket Category **{dataname}** has been updated!";
            embed.Color = Color.Blue;
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = Utils.SloganText
            };
            await RespondAsync("", new[] { embed.Build() }, ephemeral:true);
        }





    }
}
