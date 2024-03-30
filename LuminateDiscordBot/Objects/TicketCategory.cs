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
        public string CategoryName { get; set; }
        public string CategoryAliasList { get; set; }
        public string TicketDataName { get; set; }
        public string TicketDataDescription { get; set; }
        public string TicketDataAutoResponse { get; set; }

        public List<string> GetCategoryAliases()
        {
            return JsonSerializer.Deserialize<List<string>>(Convert.FromBase64String(this.CategoryAliasList));
        }
    }
}
