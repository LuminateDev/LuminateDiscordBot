using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    [CommandContextType(Discord.InteractionContextType.Guild)]
    public class UserCommands : InteractionModuleBase
    {
        private readonly DataContext _dataContext;
        public UserCommands(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
    }
}
