using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharp​Plus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BotDiscord.Commands
{
    /// <summary>
    /// The event manager for create a strawpoll
    /// </summary>
    class PollCommands : BaseCommandModule
    {
        private static readonly string[] Emojis = 
            { ":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:"};

        [Command("poll")]
        [Description("Create a strawpoll. the argument must be given in the following way : !!poll title option1 2 3 ... 9")]
        public async Task Poll(CommandContext context,
            [Description("Title of the poll")] string title,
            [Description("The options")] params string[] strawpollOptions)
        {
            List<DiscordEmoji> emojiUse = new();

            var buffer = String.Empty;
            for (int i = 0; i < strawpollOptions.Length; i++)
            {
                buffer += (i < Emojis.Length) ? $"{Emojis[i]} : {strawpollOptions[i]} \n\n" :
                "Error : Max 9 options \n";
                emojiUse.Add(DiscordEmoji.FromName(context.Client, Emojis[i]));
            }

            DiscordEmbedBuilder pollEmbed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = buffer
            };

            DiscordMessage pollMessage = await context.Channel.SendMessageAsync(embed: pollEmbed);

            foreach(DiscordEmoji emoji in emojiUse)
                await pollMessage.CreateReactionAsync(emoji);
        }
    }
}
