using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BotDiscord.Commands
{
    /// <summary>
    /// This class containt all the event Manager for
    /// the "easy" to use commandes. I created this file
    /// not only to add fun stuff to the bot but as a way
    /// to learn how to create some simple commands.
    /// </summary>
    public class ExempleCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Return Pong")]
        public async Task Ping(CommandContext context)
            => await context.Channel.SendMessageAsync("pong");

        [Command("namechannel")]
        [Description("return the name of the channel where the command is fired")]
        public async Task NameChannel(CommandContext context)
            => await context.Channel.SendMessageAsync(context.Channel.Name);

        [Command("hello")]
        [Description("say hello to the user")]
        public async Task Hello(CommandContext context)
            => await context.RespondAsync($"Hello {context.User.Username}, I hope you have a good day");

        [Command("roll20")]
        [Description("roll a 20 face dice")]
        public async Task Roll20(CommandContext context)
            => await context.RespondAsync(new Random().Next(1, 21).ToString());

        [Command("roll")]
        [Description("roll a custom dice")]
        public async Task Roll(CommandContext context, 
            [Description("The upper limit of the roll")] int int1)
            => await context.RespondAsync(new Random().Next(1, int1+1).ToString());

        [Command("addNumber")]
        [Description("add 2 integers")]
        public async Task AddNumber(CommandContext context,
            [Description("given integers")] params int[] givenIntegers)
        {
            int buffer = 0;
            foreach (int givenInteger in givenIntegers)
                buffer += givenInteger;

            await context.RespondAsync((buffer).ToString());
        }

        [Command("playlistname")]
        [Description("Give the name of all available playlist")]
        public async Task PlaylistName(CommandContext context)
        {
            var files = Directory.GetFiles(@"..\..\..\Playlist\");
            var buffer = "Playlist Available : " + "\n";
            foreach (var file in files)
                buffer += Path.GetFileNameWithoutExtension(file) + "\n";

            await context.RespondAsync(buffer);
        }
    }
}
