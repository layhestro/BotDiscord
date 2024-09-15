using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BotDiscord.JsonHandler.PlaylistJson
{
    /// <summary>
    /// This class has the methods needed to work with the Playlist Json files.
    /// To be able to read and write in these files and extract useful data.
    /// </summary>
    class PlaylistSupervisor
    {
        private static readonly string path = @"..\..\..\Playlist\";
        private static readonly string extention = ".json";
        private readonly string pathToPlaylist;
        private Playlist currentPlaylist;

        /// <summary>
        /// Give the path to the file in which this instance of PlaylistSupervisor
        /// will work with.
        /// </summary>
        /// <param name="namePlaylist">name of the desire playlist</param>
        public PlaylistSupervisor(string namePlaylist)
        {
            this.pathToPlaylist = path + namePlaylist.ToLower() + extention;
        }

        /// <summary>
        /// Asynchronously read a json file and
        /// deserialize its content in a Playlist struct
        /// </summary>
        /// <returns></returns>
        public async Task ReadPlaylistAsync()
        {
            var buffer = string.Empty;
            try
            {
                using (StreamReader sr = new(pathToPlaylist))
                    buffer = await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            currentPlaylist = JsonConvert.DeserializeObject<Playlist>(buffer);
        }

        /// <summary>
        /// Serialize the current struct Playlist
        /// and asynchronously write it in the corresponding json file.
        /// </summary>
        /// <returns></returns>
        public async Task WritePlaylistAsync()
        {
            var buffer = JsonConvert.SerializeObject(currentPlaylist);
            try
            {
                using (StreamWriter sw = new(pathToPlaylist, false))
                        await sw.WriteLineAsync(buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Select randomly a Youtube song from the current playlist
        /// different from the previously played track
        /// </summary>
        /// <returns>A string of the URL of selected Youtube song</returns>
        public string GenerateRandomSong()
        {
            int randomIndex = 0;
            do
            {
                randomIndex = new Random().Next(0, currentPlaylist.Songs.Count);
            }
            while (randomIndex == Int32.Parse(currentPlaylist.IndexOfLastSongPlayed));
            currentPlaylist.IndexOfLastSongPlayed = randomIndex.ToString();

            return currentPlaylist.Songs[randomIndex];
        }
    }
}
