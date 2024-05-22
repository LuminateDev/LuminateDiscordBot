using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot.Objects
{
    public class ChannelConfig
    {
        public required string ChannelIdentifier { get; set; }
        public ulong ChannelId { get; set; }
    }
}
