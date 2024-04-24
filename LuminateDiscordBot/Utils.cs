using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using System.Text.Json;

namespace LuminateDiscordBot
{
    internal class Utils
    {
        public static Objects.Config Config;


        public static Dictionary<string, ulong> ChannelConfig = new Dictionary<string, ulong>();
        public static Dictionary<string, ulong> RoleConfig = new Dictionary<string, ulong>();

        public static void FileCheck()
        {
            if(!Directory.Exists("LuminateConfig")) { CreateFiles(); }
            if(!File.Exists("LuminateConfig/config.json")) { CreateFiles(); }
        }

        public static void ReadFiles()
        {
            Config = JsonSerializer.Deserialize<Objects.Config>(File.ReadAllText("LuminateConfig/config.json"));
        }

        static void CreateFiles()
        {
            Directory.CreateDirectory("LuminateConfig");
            using (StreamWriter sw = File.CreateText("LuminateConfig/config.json")) { sw.Write(JsonSerializer.Serialize(new Objects.Config(), new JsonSerializerOptions { WriteIndented = true })); }
        }

        static void WriteConfig()
        {
            using (StreamWriter sw = File.CreateText("LuminateConfig/config.json")) { sw.Write(JsonSerializer.Serialize(Config), new JsonSerializerOptions() { WriteIndented = true }); }
        }



        public async static Task<ITextChannel> CreateTicketChannel(InteractionModuleBase interaction)
        {
            ITextChannel channel = await interaction.Context.Guild.CreateTextChannelAsync(Guid.NewGuid().ToString(), c => c.CategoryId = Utils.ChannelConfig["ticket_category"]);
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.EveryoneRole, OverwritePermissions.DenyAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.Guild.GetRole(Utils.RoleConfig["ticket_role"]), OverwritePermissions.AllowAll(channel));
            await channel.AddPermissionOverwriteAsync(interaction.Context.User, OverwritePermissions.AllowAll(channel));
            return channel;
        }
    }
}
