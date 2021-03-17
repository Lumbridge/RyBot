using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RyBot.Models
{
    public class GoogleCustomSearchModel
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("url")]
        public Url Url { get; set; }

        [JsonProperty("queries")]
        public Queries Queries { get; set; }

        [JsonProperty("context")]
        public Context Context { get; set; }

        [JsonProperty("searchInformation")]
        public SearchInformation SearchInformation { get; set; }

        [JsonProperty("items")]
        public List<GoogleSearchResult> SearchResults { get; set; }
    }

    public class Url
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("template")]
        public string Template { get; set; }
    }

    public class Request
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("totalResults")]
        public string TotalResults { get; set; }

        [JsonProperty("searchTerms")]
        public string SearchTerms { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("inputEncoding")]
        public string InputEncoding { get; set; }

        [JsonProperty("outputEncoding")]
        public string OutputEncoding { get; set; }

        [JsonProperty("safe")]
        public string Safe { get; set; }

        [JsonProperty("cx")]
        public string Cx { get; set; }

        [JsonProperty("searchType")]
        public string SearchType { get; set; }
    }

    public class NextPage
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("totalResults")]
        public string TotalResults { get; set; }

        [JsonProperty("searchTerms")]
        public string SearchTerms { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("inputEncoding")]
        public string InputEncoding { get; set; }

        [JsonProperty("outputEncoding")]
        public string OutputEncoding { get; set; }

        [JsonProperty("safe")]
        public string Safe { get; set; }

        [JsonProperty("cx")]
        public string Cx { get; set; }

        [JsonProperty("searchType")]
        public string SearchType { get; set; }
    }

    public class Queries
    {
        [JsonProperty("request")]
        public List<Request> Request { get; set; }

        [JsonProperty("nextPage")]
        public List<NextPage> NextPage { get; set; }
    }

    public class Context
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public class SearchInformation
    {
        [JsonProperty("searchTime")]
        public double SearchTime { get; set; }

        [JsonProperty("formattedSearchTime")]
        public string FormattedSearchTime { get; set; }

        [JsonProperty("totalResults")]
        public string TotalResults { get; set; }

        [JsonProperty("formattedTotalResults")]
        public string FormattedTotalResults { get; set; }
    }

    public class Image
    {
        [JsonProperty("contextLink")]
        public string ContextLink { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("byteSize")]
        public int ByteSize { get; set; }

        [JsonProperty("thumbnailLink")]
        public string ThumbnailLink { get; set; }

        [JsonProperty("thumbnailHeight")]
        public int ThumbnailHeight { get; set; }

        [JsonProperty("thumbnailWidth")]
        public int ThumbnailWidth { get; set; }
    }

    public class GoogleSearchResult
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("htmlTitle")]
        public string HtmlTitle { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("displayLink")]
        public string DisplayLink { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("htmlSnippet")]
        public string HtmlSnippet { get; set; }

        [JsonProperty("mime")]
        public string Mime { get; set; }

        [JsonProperty("fileFormat")]
        public string FileFormat { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }
    }
}
