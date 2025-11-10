using KwikNesta.Contracts.Models;
using KwikNesta.Mediatrix.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace KwikNesta.Property.Svc.Application.Commands
{
    public class UploadPropertyMediaCommand : IKwikRequest<ApiResult<string>>
    {
        public Guid PropertyId { get; set; }
        public List<IFormFile> Images { get; set; } = [];
        public IFormFile? Video { get; set; }
    }
}