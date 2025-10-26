using KwikNesta.Contracts.DTOs;
using Refit;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface ILocationClientService
    {
        [Get("/api/v1/locations/countries/{id}")]
        Task<ApiResponse<CountryDto>> GetCountryAsyncV1(Guid id);
    }
}