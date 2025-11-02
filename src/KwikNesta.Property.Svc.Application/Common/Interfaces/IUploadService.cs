using Hangfire;
using Hangfire.Server;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IUploadService
    {
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        Task UploadPropertyMediaAsync(List<string> imageFilePaths, string videoFilePath, Guid propertyId, PerformContext context);
    }
}
