namespace KwikNesta.Property.Svc.Application.Commands
{
    public class PropertyFeaturesRequest
    {
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int AreaSize { get; set; }
        public string AreaSizeUnit { get; set; } = "sqm";
        public bool HasParking { get; set; }
        public bool HasWaterSupply { get; set; }
        public bool HasElectricity { get; set; }
    }
}
