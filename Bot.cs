using BotDiscord.Commands;
using BotDiscord.JsonHandler.ConfigJson;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BotDiscord
{
    class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        /// <summary>
        /// This is the "hearth" of my bot. This function will :
        /// 1) Create and setup a bot client.
        /// 2) Add the custom commands created for this bot.
        /// 3) Enable the use of interactivity module.
        /// 4) Enable set up and enable the use of lavalink
        /// 5) Wait indefinitely
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            var configJson = await ConfigSupervisor.GetTokenAndCommandsPrefix();

            var config = new DiscordConfiguration
            {
                AutoReconnect = true,
                HttpTimeout = TimeSpan.FromSeconds(15),
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
                MinimumLogLevel = LogLevel.Debug,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
            };

            Client = new DiscordClient(config);

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<ExempleCommands>();
            Commands.RegisterCommands<PollCommands>();
            Commands.RegisterCommands<MusicCommands>();

            // Enable Interactivity for this client
            Client.UseInteractivity(new InteractivityConfiguration());

            // Enable and set-up Lavalink for this client
            var lavalink = Client.UseLavalink();
            var lavalinkConfig = SetUpLavaLink();

            // Connect the bot to discord 
            await Client.ConnectAsync();

            // Connect Lavalink to the bot
            await lavalink.ConnectAsync(lavalinkConfig);

            // Wait indefinitely
            await Task.Delay(-1);
        }

        /// <summary>
        /// Set up the node which Lavalink need to be use.
        /// Since I will use the bot only from my PC,
        /// hosting it locally is enough.
        /// </summary>
        /// <returns></returns>
        private LavalinkConfiguration SetUpLavaLink()
        {
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1", // From your server configuration.
                Port = 2333 // From your server configuration
            };
            var config = new LavalinkConfiguration
            {
                Password = "youshallnotpass", // From your server configuration.
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            return config;
        }
    }
}
