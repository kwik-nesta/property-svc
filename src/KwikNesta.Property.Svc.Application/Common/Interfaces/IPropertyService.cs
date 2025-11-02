using Hangfire;
using Hangfire.Server;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IPropertyService
    {
        bool IsStockImageAsync(string imageUrl);
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        Task UploadPropertyMediaAsync(List<string> imageFilePaths, string videoFilePath, Guid propertyId, PerformContext context);
        Task VerifyPropertyDetails(Guid id, PerformContext context);
        Task VerifyPropertyOwnerShip(Guid id, string userId, PerformContext context);
    }
}
