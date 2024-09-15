using Newtonsoft.Json;

namespace BotDiscord.JsonHandler.ConfigJson
{
    /// <summary>
    /// This struct is use as a container for my Playlist Json files.
    /// </summary>
    public struct Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
    }
}
