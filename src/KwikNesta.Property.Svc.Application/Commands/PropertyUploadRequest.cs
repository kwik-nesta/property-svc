using Microsoft.AspNetCore.Http;

namespace KwikNesta.Property.Svc.Application.Commands
{
    public class PropertyUploadRequest
    {
        public IFormFile? Video {  get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
}