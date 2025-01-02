using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Models
{
    public class TicketManagementModalModel : IModal
    {
        public string Title => "Modify or Create Ticket Category";

        [RequiredInput(true)]
        [InputLabel("Ticket Topic Name")]
        [ModalTextInput("name", Discord.TextInputStyle.Short, "example: I'm having issues with my Luminate account...")]
        public required string TopicName { get; set; }

        [RequiredInput(true)]
        [InputLabel("Ticket Topic Keywords")]
        [ModalTextInput("keywords", Discord.TextInputStyle.Short, "Enter one or more keywords, seperated by comma")]
        public required string TopicKeywords { get; set; }

        [RequiredInput(false)]
        [InputLabel("Ticket Topic Auto-Response")]
        [ModalTextInput("autoresponse", Discord.TextInputStyle.Paragraph, "If filled out, it will not create tickets and rather respond with the given text")]
        public string? TopicAutoResponse { get; set; } = null;

    }
}
