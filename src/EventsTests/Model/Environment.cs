using System.Text.Json.Serialization;

namespace EventsTests.Model
{
    internal class Environment
    {
        [JsonPropertyName("gtmId")]
        public string GtmId { get; set; }
        [JsonPropertyName("supportedEvents")]
        public IList<string> SupportedEvents {  get; set; }
    }
}
