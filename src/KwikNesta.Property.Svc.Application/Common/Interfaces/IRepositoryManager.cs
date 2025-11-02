namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IRepositoryManager
    {
        IPropertyRepository Property {  get; }
        IPropertyMediaRepository PropertyMedia { get; }

        Task<bool> SaveAsync(CancellationToken cancellation = default);
    }
}