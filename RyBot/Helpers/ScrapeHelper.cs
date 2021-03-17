using RyBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RyBot.Helpers
{
    public static class ScrapeHelper
    {
        public static async Task<GoogleCustomSearchModel> GetImageSearchResult(string term, string apiKey, string searchEngineContextCode, int startIndex = 0)
        {
            var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={searchEngineContextCode}&searchType=image&start={startIndex}&q={WebUtility.HtmlEncode(term)}";
            try
            {
                return await JsonHelper.GetJsonData<GoogleCustomSearchModel>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
