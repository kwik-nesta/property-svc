using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Infrastructure.Persistence;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly Lazy<IPropertyRepository> _propertyRepository;
        private readonly Lazy<IPropertyMediaRepository> _propertyMediaRepository;

        private readonly AppDbContext _context;

        public RepositoryManager(AppDbContext context)
        {
            _context = context;

            _propertyRepository = new Lazy<IPropertyRepository>(() 
                => new PropertyRepository(context));
            _propertyMediaRepository = new Lazy<IPropertyMediaRepository>(()
                => new PropertyMediaRepository(context));
        }

        public IPropertyRepository Property => _propertyRepository.Value;
        public IPropertyMediaRepository PropertyMedia => _propertyMediaRepository.Value;

        public async Task<bool> SaveAsync(CancellationToken cancellation = default) =>
            await _context.SaveChangesAsync(cancellation) > 0;
    }
}