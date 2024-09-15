using Newtonsoft.Json;
using System.Collections.Generic;

namespace BotDiscord.JsonHandler.PlaylistJson
{   
    /// <summary>
    /// This struct is use as a container for my Playlist Json files.
    /// </summary>
    public struct Playlist
    {
        [JsonProperty("playlistTitle")]
        public string PlaylistTitle { get; private set; }

        private string indexOfLastSongPlayed;
        [JsonProperty("indexOfLastSongPlayed")]
        public string IndexOfLastSongPlayed
        {
            get => indexOfLastSongPlayed;
            set => indexOfLastSongPlayed = value;
        }

        [JsonProperty("songs")]
        public List<string> Songs { get; private set; }
    }
}
