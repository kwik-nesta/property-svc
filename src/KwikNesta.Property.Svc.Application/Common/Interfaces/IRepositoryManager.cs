namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IRepositoryManager
    {
        IPropertyRepository Property {  get; }
        IPropertyMediaRepository PropertyMedia { get; }
        IPropertyLocationRepository PropertyLocation { get; }

        Task<bool> SaveAsync(CancellationToken cancellation = default);
    }
}