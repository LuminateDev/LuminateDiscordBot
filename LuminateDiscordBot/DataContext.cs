using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    public class DataContext : DbContext
    {
        public DbSet<Models.Database.DataConfig> DataConfigs { get; set; }
        public DbSet<Models.Database.TicketCategory> TicketCategories { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
