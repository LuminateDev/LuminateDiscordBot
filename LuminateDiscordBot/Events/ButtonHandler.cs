using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LuminateDiscordBot.Objects;

namespace LuminateDiscordBot.Events;

public class ButtonHandler
{
    
    public static async Task HandleButtonEvent(SocketMessageComponent component)
    {
        
        switch (component.Data.CustomId)
        {
            case "changeName":
                try
                {
                    await component.RespondWithModalAsync<Modals>("new_name_input");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
        }
    }
}