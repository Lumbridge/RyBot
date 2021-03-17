using System.Collections.Generic;
using Newtonsoft.Json;

namespace RyBot.Models
{
    public class TraceMoeApiResponseModel
    {
        [JsonProperty("RawDocsCount")]
        public string RawDocsCount { get; set; }

        [JsonProperty("RawDocsSearchTime")]
        public string RawDocsSearchTime { get; set; }

        [JsonProperty("ReRankSearchTime")]
        public string ReRankSearchTime { get; set; }

        [JsonProperty("CacheHit")]
        public bool CacheHit { get; set; }

        [JsonProperty("trial")]
        public string Trial { get; set; }

        [JsonProperty("limit")]
        public string Limit { get; set; }

        [JsonProperty("limit_ttl")]
        public string LimitTtl { get; set; }

        [JsonProperty("quota")]
        public string Quota { get; set; }

        [JsonProperty("quota_ttl")]
        public string QuotaTtl { get; set; }

        [JsonProperty("docs")]
        public List<Doc> Docs { get; set; }
    }

    public class Doc
    {
        [JsonProperty("from")]
        public double From { get; set; }

        [JsonProperty("to")]
        public double To { get; set; }

        [JsonProperty("anilist_id")]
        public string AnilistId { get; set; }

        [JsonProperty("at")]
        public double At { get; set; }

        [JsonProperty("season")]
        public string Season { get; set; }

        [JsonProperty("anime")]
        public string Anime { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("episode")]
        public string Episode { get; set; }

        [JsonProperty("tokenthumb")]
        public string Tokenthumb { get; set; }

        [JsonProperty("similarity")]
        public double Similarity { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_native")]
        public string TitleNative { get; set; }

        [JsonProperty("title_chinese")]
        public string TitleChinese { get; set; }

        [JsonProperty("title_english")]
        public string TitleEnglish { get; set; }

        [JsonProperty("title_romaji")]
        public string TitleRomaji { get; set; }

        [JsonProperty("mal_id")]
        public string MalId { get; set; }

        [JsonProperty("synonyms")]
        public List<string> Synonyms { get; set; }

        [JsonProperty("synonyms_chinese")]
        public List<object> SynonymsChinese { get; set; }

        [JsonProperty("is_adult")]
        public bool IsAdult { get; set; }
    }
}
