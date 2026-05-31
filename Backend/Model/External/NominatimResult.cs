using System.Text.Json.Serialization;

namespace AA2_CS.Model.External
{
    public class NominatimResult
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; }

        [JsonPropertyName("lon")]
        public string Lon { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        public NominatimResult() { }

        public NominatimResult(string lat, string lon)
        {
            Lat = lat;
            Lon = lon;
        }
    }
}
