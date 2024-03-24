using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Events
{
    internal class InteractionHandler
    {
        public static async Task HandleInteraction(SocketInteraction interaction)
        {
            SocketInteractionContext context = new SocketInteractionContext(Program.client, interaction);
            IResult result = await Program._interactionService.ExecuteCommandAsync(context, Program._services);
        }
    }
}
