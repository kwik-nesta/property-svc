namespace KwikNesta.Property.Svc.Application.Commands
{
    public class PropertyLocationRequest
    {
        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
    }
}
