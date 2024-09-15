using System.Threading.Tasks;

namespace BotDiscord
{
    class Program
    {
        static async Task Main()
        {
            Bot bot = new();
            await bot.RunAsync();
        }
    }
}