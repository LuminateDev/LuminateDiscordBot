using Discord;

namespace LuminateDiscordBot
{
    internal class Responses
    {
        public static Embed NoPermissions()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Color = Color.Red;
            embed.Title = "No permissions!";
            embed.Description = "It seems that you dont have the required permissions to access this command.";
            embed.Footer = new EmbedFooterBuilder()
            {
                Text = $"Luminate - Your ideas shine bright"
            };
            return embed.Build();
        }

        public static Embed TicketInitMessage(string topic, string issue, ulong user)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Incoming Ticket!";
            embed.Color = Color.Blue;
            embed.Description = "Your ticket has been created, a Team Luminate member will take a look at it shortly.";
            embed.AddField("Topic", topic);
            embed.AddField("Issue", issue);
            embed.AddField("Ticket Author", $"<@{user}>");
            return embed.Build();
        }
    }
}
