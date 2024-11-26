using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    [Group("staff", "Contains all staff commands")]
    [RequireUserPermission(Discord.GuildPermission.Administrator)]
    [CommandContextType(Discord.InteractionContextType.Guild)]
    public class StaffCommands :InteractionModuleBase
    {
        private readonly DataContext _dataContext;
        public StaffCommands(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
    }
}
