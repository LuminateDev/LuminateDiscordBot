using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Objects
{
    public class Config
    {
        [JsonPropertyName("bot_token")] public string BotToken { get; set; } = "";
    }
}
