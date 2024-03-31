using System.Xml;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace LuminateDiscordBot.Events;

internal class UserJoinVC
{
    public static async Task HandleUserJoinVoice(SocketUser user, SocketVoiceState state, SocketVoiceState state2)
    {
        var channel = state.VoiceChannel;
        var channelId = DBManager.GetPrivateJoinVoiceChannel(channel.Guild.Id);
        Console.Out.WriteLine(channelId);
        Console.Out.WriteLine(channel.Id);
        if (channel.Id == channelId)
        {
            var cat = channel.Category.Id;
            Console.Out.WriteLine(cat);
            SocketGuild guild = channel.Guild;
            SocketGuildUser guildUser = state.VoiceChannel.GetUser(user.Id);
            Console.Out.WriteLine("Started");
            var privateVc = await guild.CreateVoiceChannelAsync(guildUser.Username + "'s Channel");
            Console.Out.WriteLine("Done");
            var lockChannel = new OverwritePermissions(viewChannel: PermValue.Deny);
            var ownerPermission = new OverwritePermissions(manageChannel: PermValue.Allow);
            await privateVc.AddPermissionOverwriteAsync(guild.EveryoneRole, lockChannel);
            await privateVc.AddPermissionOverwriteAsync(guild.GetUser(user.Id), ownerPermission);
            await guild.MoveAsync(guildUser, privateVc);
        }
    }
}