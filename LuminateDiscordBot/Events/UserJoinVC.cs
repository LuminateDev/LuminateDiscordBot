using Discord;
using Discord.WebSocket;

namespace LuminateDiscordBot.Events;

public class UserJoinVC
{
    public static async Task HandleUserJoinVoice(SocketVoiceChannel channel, SocketGuildUser user)
    {
        ulong channelId = DBManager.GetPrivateJoinVoiceChannel(user.Guild.Id);
        if (channel.Id == channelId)
        {
            ulong cat = channel.Category.Id;
            SocketGuild guild = user.Guild;
            await user.Guild.CreateVoiceChannelAsync(user.Nickname + "'s Channel", tcp => tcp.CategoryId = cat);
            //guild.GetVoiceChannel(guild.Channels.First(c => c.Name == user.Nickname + "'s Channel").AddPermissionOverwriteAsync(guild.EveryoneRole, OverwritePermissions.DenyAll(""));
        }
    }
}