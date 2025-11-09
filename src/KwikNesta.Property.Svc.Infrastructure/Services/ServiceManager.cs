using Cloudtenary.Abstract;
using CrossQueue.Hub.Services.Interfaces;
using KwikNesta.Property.Svc.Application.Common.Interfaces;

namespace KwikNesta.Property.Svc.Infrastructure.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IPropertyService> _uploadService;

        public ServiceManager(ICloudtenary cloudtenary,
                              IRepositoryManager repository,
                              IRabbitMQPubSub rabbitMQ,
                              IReverseGeocodeService reverseGeocode)
        {
            _uploadService = new Lazy<IPropertyService>(() =>
                new PropertyService(cloudtenary, repository, rabbitMQ, reverseGeocode));
        }

        public IPropertyService Upload => _uploadService.Value;
    }
}