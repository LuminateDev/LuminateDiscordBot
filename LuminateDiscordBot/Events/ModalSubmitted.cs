using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;

namespace LuminateDiscordBot.Events
{
    public class ModalSubmitted
    {
        public static async Task HandleModalSubmitEvent(SocketModal modal)
        {
            switch (modal.Data.CustomId)
            {
                case "changeNameModal":
                    await HandleChangeChannelNameModal(modal);
                    break;
                case "changeLimitModal":
                    await HandleChangeUserLimitModal(modal);
                    break;
                default:
                    await modal.RespondAsync("Unknown modal submitted.", ephemeral: true);
                    break;
            }
        }

        private static async Task HandleChangeChannelNameModal(SocketModal modal)
        {
            var newChannelName = modal.Data.Components.FirstOrDefault(x => x.CustomId == "channelName")?.Value;

            if (string.IsNullOrWhiteSpace(newChannelName))
            {
                await modal.RespondAsync("You must provide a new channel name.", ephemeral: true);
            }
            else
            {
                await modal.RespondAsync($"Channel name will be changed to: {newChannelName}", ephemeral: true);
            }
        }

        private static async Task HandleChangeUserLimitModal(SocketModal modal)
        {
            var newUserLimit = modal.Data.Components.FirstOrDefault(x => x.CustomId == "maxLimit")?.Value;

            if (string.IsNullOrWhiteSpace(newUserLimit))
            {
                await modal.RespondAsync("You must provide a new user limit.", ephemeral: true);
            }
            else
            {
                await modal.RespondAsync($"User limit will be changed to: {newUserLimit}", ephemeral: true);
            }
        }
    }
}
