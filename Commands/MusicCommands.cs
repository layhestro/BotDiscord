using BotDiscord.JsonHandler.PlaylistJson;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BotDiscord.Commands
{
    /// <summary>
    /// The events managers for all the commands which use Lavalink and
    /// some methodes to handle errors as well.
    /// </summary>
    class MusicCommands : BaseCommandModule
    {
        /// <summary>
        /// Check if Lavalink is connect to the client 
        /// and return the first node in which lavalink is connected
        /// </summary>
        /// <param name="context">Represents a context in which a command is executed.</param>
        /// <returns></returns>
        private async Task<LavalinkNodeConnection> GetNode(CommandContext context)
        {
            var lava = context.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await context.RespondAsync("The Lavalink connection is not established");
                return null;
            }

            return lava.ConnectedNodes.Values.First();
        }

        /// <summary>
        /// Check if the channel given is a voicechannel.
        /// </summary>
        /// <param name="context">Represents a context in which a command is executed.</param>
        /// <param name="channel">Name of the channel given.</param>
        /// <returns></returns>
        private async Task<bool> IsAVoiceChannel(CommandContext context, DiscordChannel channel)
        {
            if (channel.Type != ChannelType.Voice)
            {
                await context.RespondAsync("Not a valid voice channel.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// the current active lavalink connection
        /// </summary>
        /// <param name="context">Represents a context in which a command is executed.</param>
        /// <param name="node">the first node in which lavalink is connected</param>
        /// <param name="channel">Name of the channel given.</param>
        /// <returns>Return current active lavalink connection</returns>
        private async Task<LavalinkGuildConnection> GetCurrentConnection
            (CommandContext context,
            LavalinkNodeConnection node, 
            DiscordChannel channel)
        {
            var conn = node.GetGuildConnection(channel.Guild);
            if (conn == null)
            {
                await context.RespondAsync("Lavalink is not currently connected.");
                return null;
            }
            return conn;
        }

        /// <summary>
        /// Check if the user sending the commands is in a voice channel
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true if the user is in a voice channel, false otherwise</returns>
        private async Task<bool> MemberInVoiceChannel(CommandContext context)
        {
            if (context.Member.VoiceState is null || context.Member.VoiceState.Channel is null)
            {
                await context.RespondAsync("You are not in a voice channel.");
                return false;
            }
            return true;
        }

        [Command("join")]
        [Description("The bot join the vocal channel mentions")]
        public async Task Join(CommandContext context, 
            [Description("The name of the channel you are currently in")] DiscordChannel channel)
        {
            if (!(await IsAVoiceChannel(context, channel)))
                return;

            var node = await GetNode(context);
            if (node == null)
                return;

            await node.ConnectAsync(channel);
            await context.RespondAsync($"Joined {channel.Name}!");
        }

        [Command("joinme")]
        [Description("The bot join your current vocal channel")]
        public async Task JoinMe(CommandContext context)
        {
            var myChannel = context.Member.VoiceState.Channel;
            await Join(context, myChannel);
        }

        [Command("leave")]
        [Description("The bot leave the vocal channel mentions")]
        public async Task Leave(CommandContext context,
            [Description("The name of the channel the bot is currently in")] DiscordChannel channel)
        {
            if (!(await IsAVoiceChannel(context, channel)))
                return;

            var node = await GetNode(context);
            if (node == null)
                return;

            var conn = await GetCurrentConnection(context, node, channel);
            if (conn == null)
                return;
            
            await conn.DisconnectAsync();
            await context.RespondAsync($"Left {channel.Name}!");
        }

        [Command("leaveme")]
        [Description("The bot leave your current vocal channel")]
        public async Task LeaveMe(CommandContext context)
        {
            var myChannel = context.Member.VoiceState.Channel;
            await Leave(context, myChannel);
        }

        [Command("play")]
        [Description("The bot play the youtube video of given URL")]
        public async Task Play(CommandContext context,
            [Description("URL of desired video")] Uri url)
        {
            if (!(await MemberInVoiceChannel(context)))
                return;

            var node = await GetNode(context);
            if (node == null)
                return;

            var currentChannel = context.Member.VoiceState.Channel;
            var conn = await GetCurrentConnection(context, node, currentChannel);
            if (conn == null)
                return;

            var loadResult = await node.Rest.GetTracksAsync(url);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await context.RespondAsync($"Track search failed for {url}.");
                return;
            }

            var track = loadResult.Tracks.First();
            await conn.PlayAsync(track);
            await context.RespondAsync($"Now playing {track.Title}!");
        }

        [Command("pause")]
        [Description("The bot pause the track currently playing")]
        public async Task Pause(CommandContext context)
        {
            if (!(await MemberInVoiceChannel(context)))
                return;

            var node = await GetNode(context);
            if (node == null)
                return;

            var currentChannel = context.Member.VoiceState.Channel;
            var conn = await GetCurrentConnection(context, node, currentChannel);
            if (conn == null)
                return;

            if (conn.CurrentState.CurrentTrack == null)
            {
                await context.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.PauseAsync();
        }

        [Command("resume")]
        [Description("The bot resume the track currently being paused")]
        public async Task Resume(CommandContext context)
        {
            if (!(await MemberInVoiceChannel(context)))
                return;

            var node = await GetNode(context);
            if (node == null)
                return;

            var currentChannel = context.Member.VoiceState.Channel;
            var conn = await GetCurrentConnection(context, node, currentChannel);
            if (conn == null)
                return;

            if (conn.CurrentState.CurrentTrack == null)
            {
                await context.RespondAsync("There are no tracks loaded.");
                return;
            }

            await conn.ResumeAsync();
        }

        [Command("p")]
        [Description("Launch a random song from the playlist called")]
        public async Task Playlist(CommandContext context, string namePlaylist)
        {
            PlaylistSupervisor supervisor = new(namePlaylist);
            await supervisor.ReadPlaylistAsync();
            await Play(context, new Uri(supervisor.GenerateRandomSong()));
            await supervisor.WritePlaylistAsync();
        }
    }
}
