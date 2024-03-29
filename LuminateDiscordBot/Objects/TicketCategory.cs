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
        public List<string> CategoryAliasList { get; set; }


        public TicketCategory(string Catname, string aliasB64)
        {
            this.CategoryName = Catname;
            this.CategoryAliasList = JsonSerializer.Deserialize<List<string>>(Convert.FromBase64String(aliasB64));
        }
    }
}
