using System.Xml;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace LuminateDiscordBot.Events;

internal class UserJoinVC
{
    public static async Task HandleUserJoinVoice(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        var channel = after.VoiceChannel;
        var channelId = DBManager.GetPrivateJoinVoiceChannel(channel.Guild.Id);
        Console.Out.WriteLine(channelId);
        Console.Out.WriteLine(channel.Id);
        if (user is SocketGuildUser socketUser)
        {
            if (channel.Id == channelId)
            {
                SocketGuild guild = before.VoiceChannel.Guild;
                ulong catId = DBManager.GetCategoryId(guild.Id);
                Console.Out.WriteLine("Started");
                var privateVc = await guild.CreateVoiceChannelAsync(socketUser.Username + "'s Channel", tcp => tcp.CategoryId = catId);
                Console.Out.WriteLine("Done");
                DBManager.AddVoiceChannel(privateVc.Id, guild.Id, user.Id);
                var lockChannel = new OverwritePermissions(viewChannel: PermValue.Deny);
                var ownerPermission = new OverwritePermissions(manageChannel: PermValue.Allow);
                await privateVc.AddPermissionOverwriteAsync(guild.EveryoneRole, lockChannel);
                await privateVc.AddPermissionOverwriteAsync(guild.GetUser(user.Id), ownerPermission);
                await guild.MoveAsync(socketUser, privateVc);
            }
            
            Console.Out.Write("Called!");
            if (after.VoiceChannel != before.VoiceChannel
                && before.VoiceChannel.ConnectedUsers.Count == 0
                && DBManager.SearchPrivateVoiceChannels(before.VoiceChannel.Guild.Id, before.VoiceChannel.Id))
            {
                Console.Out.Write("Deleting channel...");
                await before.VoiceChannel.DeleteAsync();
                DBManager.DeleteVoiceChannel(before.VoiceChannel.Guild.Id);
            }
        }
    }
}