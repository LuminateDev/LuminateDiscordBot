using Discord;

namespace LuminateDiscordBot.Objects;

public class Modals
{
    public static ModalBuilder ChangeChannelNameModal()
    {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Change Channel Name")
            .WithCustomId("changeNameModal")
            .AddTextInput("New Channel Name", "channelName", placeholder: "Enter the new channel name here");

        return modalBuilder;
    }

    public static ModalBuilder ChangeUserLimit()
    {
        var modalBuilder = new ModalBuilder()
            .WithTitle("Change Max Limit")
            .WithCustomId("changeLimitModal")
            .AddTextInput("Change User Limit", "maxLimit", placeholder: "Enter the new max amount of users");

        return modalBuilder;
    }
}