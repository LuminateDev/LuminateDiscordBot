using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System.Text.Json;

namespace LuminateDiscordBot
{
    public class Utils
    {

        public Dictionary<string, ulong> ChannelConfig = new Dictionary<string, ulong>();
        public Dictionary<string, ulong> RoleConfig = new Dictionary<string, ulong>();

        public Task FileCheck()
        {
            if (!Directory.Exists(Constants.APP_ROOT)) { CreateFiles(); }
            if (!File.Exists($"{Constants.APP_ROOT}/config.json")) { CreateFiles(); }
            return Task.CompletedTask;
        }

        public Objects.Config GetConfig() => JsonSerializer.Deserialize<Objects.Config>(File.ReadAllText($"{Constants.APP_ROOT}/config.json"))!;

        private Task CreateFiles()
        {
            Directory.CreateDirectory(Constants.APP_ROOT);
            using (StreamWriter sw = File.CreateText($"{Constants.APP_ROOT}/config.json")) { sw.Write(JsonSerializer.Serialize(new Objects.Config(), new JsonSerializerOptions { WriteIndented = true })); }
            return Task.CompletedTask;
        }

        public Task ReloadChannelConfig(List<Models.Database.DataConfig> data)
        {
            Dictionary<string, ulong> _config = new Dictionary<string, ulong>();
            foreach(var entry in data)
            {
                _config.Add(entry.DataName, entry.DataValue);
            }
            ChannelConfig = _config;
            return Task.CompletedTask;
        }

        public Task ReloadRoleConfig(List<Models.Database.DataConfig> data)
        {
            Dictionary<string, ulong> _config = new Dictionary<string, ulong>();
            foreach(var entry in data)
            {
                _config.Add(entry.DataName, entry.DataValue);
            }
            RoleConfig = _config;
            return Task.CompletedTask;
        }

        public async Task<ITextChannel> CreateTicketChannel(InteractionModuleBase interaction)
        {
            ITextChannel channel = await interaction.Context.Guild.CreateTextChannelAsync(Guid.NewGuid().ToString(), c => c.CategoryId = this.ChannelConfig[Constants.TICKET_CATEGORY_IDENTIFIER]);
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.GetRole(this.RoleConfig[Constants.TICKET_ROLE_IDENTIFIER]), OverwritePermissions.AllowAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.User, OverwritePermissions.InheritAll);
            return channel;
        }
    }
}
