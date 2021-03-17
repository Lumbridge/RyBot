using Newtonsoft.Json;

namespace RyBot.Models
{
    public class TraceMoeApiUsageModel
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("limit_ttl")]
        public long LimitTtl { get; set; }

        [JsonProperty("quota")]
        public long Quota { get; set; }

        [JsonProperty("quota_ttl")]
        public long QuotaTtl { get; set; }

        [JsonProperty("user_limit")]
        public long UserLimit { get; set; }

        [JsonProperty("user_limit_ttl")]
        public long UserLimitTtl { get; set; }

        [JsonProperty("user_quota")]
        public long UserQuota { get; set; }

        [JsonProperty("user_quota_ttl")]
        public long UserQuotaTtl { get; set; }
    }
}