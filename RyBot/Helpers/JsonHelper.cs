using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RyBot.Helpers
{
    public static class JsonHelper
    {
        public static async Task<T> GetJsonData<T>(string url) where T : new()
        {
            // ReSharper disable once ConvertToUsingDeclaration
            using (var client = new WebClient())
            {
                var jsonString = client.DownloadString(url);
                await Task.CompletedTask;
                return !string.IsNullOrEmpty(jsonString) ? JsonConvert.DeserializeObject<T>(jsonString) : new T();
            }
        }
    }
}
