using Cloudtenary.Abstract;
using CrossQueue.Hub.Services.Interfaces;
using KwikNesta.Property.Svc.Application.Common.Interfaces;

namespace KwikNesta.Property.Svc.Infrastructure.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUploadService> _uploadService;

        public ServiceManager(ICloudtenary cloudtenary,
                              IRepositoryManager repository,
                              IRabbitMQPubSub rabbitMQ)
        {
            _uploadService = new Lazy<IUploadService>(() =>
                new UploadService(cloudtenary, repository, rabbitMQ));
        }

        public IUploadService Upload => _uploadService.Value;
    }
}