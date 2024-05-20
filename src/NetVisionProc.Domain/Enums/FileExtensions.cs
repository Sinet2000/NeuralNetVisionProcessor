using System.ComponentModel;

namespace NetVisionProc.Domain.Enums
{
    public enum FileExtensions : byte
    {
        [Description(".jpg")]
        Jpg = 1,
        
        [Description(".png")]
        Png = 2,
        Gif = 3,
        Bmp = 4,
        Tiff = 5,
        Ico = 6,
        Jfif = 7,
        Webp = 8,
        Heif = 9,
        Bat = 10,
        Csv = 11,
        Docx = 12,
        Epub = 13,
        Jar = 14,
        Json = 15,
        Mp3 = 16,
        Mp4 = 17,
        Ods = 18,
        Pdf = 19,
        Xlsx = 20,
        
        [Description(".jpeg")]
        Jpeg = 21
    }
}