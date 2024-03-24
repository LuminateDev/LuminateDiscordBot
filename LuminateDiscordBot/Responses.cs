using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Text = $"Luminate - {Utils.DefaultSloganText}"
            };
            return embed.Build();
        }
    }
}
