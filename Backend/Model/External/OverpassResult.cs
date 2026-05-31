using System.Text.Json.Serialization;

namespace AA2_CS.Model.External
{
    public class OverpassResult
    {
        [JsonPropertyName("elements")]
        public List<OverpassElement> Elements { get; set; }
    }

    public class OverpassElement
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        [JsonPropertyName("center")]
        public OverpassCenter Center { get; set; }

        [JsonPropertyName("tags")]
        public OverpassTags Tags { get; set; }
    }

    public class OverpassCenter
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    public class OverpassTags
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("leisure")]
        public string Leisure { get; set; }

        [JsonPropertyName("addr:street")]
        public string Street { get; set; }

        [JsonPropertyName("addr:housenumber")]
        public string HouseNumber { get; set; }

        [JsonPropertyName("addr:postcode")]
        public string Postcode { get; set; }

        [JsonPropertyName("addr:city")]
        public string City { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("contact:phone")]
        public string ContactPhone { get; set; }

        [JsonPropertyName("website")]
        public string Website { get; set; }

        [JsonPropertyName("contact:website")]
        public string ContactWebsite { get; set; }

        [JsonPropertyName("opening_hours")]
        public string OpeningHours { get; set; }

        [JsonPropertyName("wheelchair")]
        public string Wheelchair { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; }
    }
}
