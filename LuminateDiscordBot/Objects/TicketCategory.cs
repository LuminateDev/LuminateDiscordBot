using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Objects
{
    public class TicketCategory
    {
        public required string TicketTopic { get; set; }
        public required string CategoryAliasList { get; set; }
        public required string TicketDataName { get; set; }
        public required string TicketDataDescription { get; set; }
        public required string TicketDataAutoResponse { get; set; }

        public List<string>? GetCategoryAliases()
        {
            return JsonSerializer.Deserialize<List<string>>(Convert.FromBase64String(this.CategoryAliasList));
        }
        
    }
}
