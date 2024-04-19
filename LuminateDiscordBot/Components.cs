using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    public class Components : InteractionModuleBase
    {

        [ComponentInteraction("ticket-start")]
        public async Task HandleTicketMenu()
        {
            
        }
    }
}
