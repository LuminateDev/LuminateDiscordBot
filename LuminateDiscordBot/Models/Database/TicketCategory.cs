using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Models.Database
{
    [Table("ticket_categories")]
    public class TicketCategory
    {
        [Key]
        [Column("category_id")]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString("N");

        [Column("ticket_topic")] public required string TicketTopic { get; set; }
        [Column("category_aliases")]public required string CategoryAliases { get; set; }
        [Column("ticket_data_name")]public required string TicketDataName { get; set; }
        [Column("ticket_data_description")]public required string TicketDataDescription { get; set; }
        [Column("ticket_data_auto_response")]public string? TicketDataAutoResponse { get; set; }
        [Column("auto_response_enabled")] public required bool AutoResponseEnabled { get; set; }
    }
}
