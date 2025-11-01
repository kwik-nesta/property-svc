namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IRepositoryManager
    {
        IPropertyRepository Property {  get; }

        Task<bool> SaveAsync(CancellationToken cancellation = default);
    }
}