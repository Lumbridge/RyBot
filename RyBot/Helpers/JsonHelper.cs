using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RyBot.Models;

namespace RyBot.Helpers
{
    public static class JsonHelper
    {
        public static async Task<GoogleCustomSearchModel> GetImageSearchResult(string term, string apiKey, string searchEngineContextCode, int startIndex = 0)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={searchEngineContextCode}&searchType=image&start={startIndex}&q={WebUtility.HtmlEncode(term)}";
            try
            {
                return await GetJsonData<GoogleCustomSearchModel>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

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
