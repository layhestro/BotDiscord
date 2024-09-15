using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BotDiscord.JsonHandler.ConfigJson
{
    /// <summary>
    /// This class has the methods needed to work with the Config Json files.
    /// To be able to read the file and extract useful data.
    /// </summary>
    static class ConfigSupervisor
    {
        private static readonly string pathToConfigFile = @"..\..\..\ConfigFile\config.json";

        /// <summary>
        /// Asynchronously read a json file and
        /// deserialize its content in a Playlist struct
        /// </summary>
        /// <returns></returns>
        public static async Task<Config> GetTokenAndCommandsPrefix()
        {
            var buffer = string.Empty;
            try
            {
                using (StreamReader sr = new(pathToConfigFile))
                    buffer = await sr.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return JsonConvert.DeserializeObject<Config>(buffer);
        }
    }
}
