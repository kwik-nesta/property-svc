using System.ComponentModel;

namespace KwikNesta.Property.Svc.Domain.Enums
{
    public enum PropertyType
    {
        [Description("Apartment")]
        Apartment = 1,
        [Description("House")]
        House = 2,
        [Description("Duplex")]
        Duplex = 3,
        [Description("Bungalow")]
        Bungalow = 4,
        [Description("Condominium")]
        Condo = 5,
        [Description("Villa")]
        Villa = 6,
        [Description("Townhouse")]
        Townhouse = 7,
        [Description("Studio Apartment")]
        Studio = 8,
        [Description("Penthouse")]
        Penthouse = 9,

        [Description("Office Space")]
        Office = 20,
        [Description("Retail Shop")]
        Retail = 21,
        [Description("Warehouse")]
        Warehouse = 22,
        [Description("Industrial Property")]
        Industrial = 23,
        [Description("Co-Working Space")]
        CoWorkingSpace = 24,
        [Description("Shopping Mall")]
        ShoppingMall = 25,
        [Description("Hotel / Hospitality")]
        Hotel = 26,

        [Description("Residential Land")]
        ResidentialLand = 40,
        [Description("Commercial Land")]
        CommercialLand = 41,
        [Description("Agricultural Land")]
        AgriculturalLand = 42,
        [Description("Mixed Use Land")]
        MixedUseLand = 43,

        [Description("School / Educational")]
        School = 60,
        [Description("Hospital / Healthcare")]
        Hospital = 61,
        [Description("Religious Building")]
        Religious = 62,
        [Description("Government Building")]
        GovernmentBuilding = 63,

        [Description("Other / Unspecified")]
        Unknown = 99
    }
}