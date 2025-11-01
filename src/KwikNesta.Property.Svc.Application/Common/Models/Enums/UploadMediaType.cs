using System.ComponentModel;

namespace KwikNesta.Property.Svc.Application.Common.Models.Enums
{
    public enum UploadMediaType
    {
        [Description(".png|.jpg|.jpeg")]
        Image,
        [Description(".mp4")]
        Video,
        [Description(".pdf|.doc|.docx|.xls|.xlsx")]
        Document
    }
}
