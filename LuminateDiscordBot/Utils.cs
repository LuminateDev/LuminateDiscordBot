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

        public Objects.Config GetConfig() => JsonSerializer.Deserialize<Objects.Config>(File.ReadAllText("LuminateConfig/config.json"))!;

        private Task CreateFiles()
        {
            Directory.CreateDirectory(Constants.APP_ROOT);
            using (StreamWriter sw = File.CreateText($"{Constants.APP_ROOT}/config.json")) { sw.Write(JsonSerializer.Serialize(new Objects.Config(), new JsonSerializerOptions { WriteIndented = true })); }
            return Task.CompletedTask;
        }



        public async Task<ITextChannel> CreateTicketChannel(InteractionModuleBase interaction)
        {
            ITextChannel channel = await interaction.Context.Guild.CreateTextChannelAsync(Guid.NewGuid().ToString(), c => c.CategoryId = this.ChannelConfig["ticket_category"]);
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.GetRole(this.RoleConfig["ticket_role"]), OverwritePermissions.AllowAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.User, OverwritePermissions.InheritAll);
            return channel;
        }
    }
}
