using Discord;
using Discord.Interactions;

namespace LuminateDiscordBot.Objects;

public class Modals : IModal
{
    public string Title => "Change Name";

    [InputLabel("New Name")]
    [ModalTextInput("new_name_input", placeholder: "Enter your new name", style: TextInputStyle.Short, maxLength: 100)]
    public string NewName { get; set; }
}