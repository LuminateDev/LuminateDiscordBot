using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Models
{
    public class TicketCreationModalModel : IModal
    {
        public string Title => "Create a ticket";


        [RequiredInput(true)]
        [InputLabel("State your issue")]
        [ModalTextInput("problem", Discord.TextInputStyle.Paragraph, "Please enter your issue here", 1, 1000)]
        public required string Reason { get; set; }

    }
}
