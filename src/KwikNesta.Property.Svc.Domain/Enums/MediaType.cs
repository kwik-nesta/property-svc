using System.ComponentModel;

namespace KwikNesta.Property.Svc.Domain.Enums
{
    public enum MediaType : byte
    {
        [Description("Image")]
        Image,
        [Description("Video")]
        Video,
        [Description("Document")]
        Document
    }
}