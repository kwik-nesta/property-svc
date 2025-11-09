using System.Text.Json.Serialization;

namespace KwikNesta.Property.Svc.Application.Dtos
{
    public class ReverseGeocodeResult
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; } = string.Empty;
        [JsonPropertyName("lon")]
        public string Longitude { get; set; } = string.Empty;
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
        public Address? Address { get; set; }
    }

    public class Address
    {
        public string Road { get; set; } = string.Empty;
        public string Neighbourhood { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
