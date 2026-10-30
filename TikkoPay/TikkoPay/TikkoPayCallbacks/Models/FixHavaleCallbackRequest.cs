using System.Text.Json.Serialization;

namespace TikkoPayCallbacks.Models
{
    public class FixHavaleCallbackRequest
    {
        [JsonPropertyName("event")]
        public string? Event { get; set; }

        [JsonPropertyName("eventAt")]
        public DateTimeOffset EventAt { get; set; }

        [JsonPropertyName("transaction")]
        public FixHavaleTransaction? Transaction { get; set; }
    }

    public class FixHavaleTransaction
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("providerId")]
        public string? ProviderId { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("source")]
        public FixHavaleSource? Source { get; set; }

        [JsonPropertyName("user")]
        public FixHavaleUser? User { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class FixHavaleSource
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class FixHavaleUser
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }
    }
}
