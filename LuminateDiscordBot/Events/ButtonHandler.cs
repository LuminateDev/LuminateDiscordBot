using Discord.Interactions;
using Discord.WebSocket;
using LuminateDiscordBot.Objects;

namespace LuminateDiscordBot.Events;

public class ButtonHandler
{

    public static async Task HandleButtonEvent(SocketMessageComponent component)
    {
        ulong userId = DBManager.CheckOwnershipForPrivateChannel(component.GuildId.Value);
        if (userId == component.User.Id)
        {

            switch (component.Data.CustomId)
            {
                case "changeName":
                {
                    await component.RespondWithModalAsync(Modals.ChangeChannelNameModal().Build());
                    break;
                }
                case "changeLimit":
                {
                    await component.RespondWithModalAsync(Modals.ChangeUserLimit().Build());
                    break;
                }
                case "changePrivacy":
                {
                    break;
                }
                case "trustPerson":
                {
                    break;
                }
                case "untrustPerson":
                {
                    break;
                }
                case "transferOwnership":
                {
                    break;
                }
                case "deleteChannel":
                {
                    break;
                }
            }
        }
        else
        {
            await component.RespondAsync("You don't own a channel.", ephemeral: true);
        }
    }
}