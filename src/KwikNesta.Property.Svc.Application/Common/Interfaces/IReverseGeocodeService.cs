using KwikNesta.Property.Svc.Application.Dtos;
using Refit;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IReverseGeocodeService
    {
        [Get("/reverse")]
        Task<ApiResponse<ReverseGeocodeResult>> ReverseGeocoder([Query] string lat, 
                                                  [Query] string lon, 
                                                  [Query] string format = "json");
    }
}