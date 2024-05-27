using System.ComponentModel;

namespace NetVisionProc.Common;

public enum FileExtensions
{
    [Description(".jpeg")]
    Jpeg,

    [Description(".jpg")]
    Jpg,

    [Description(".png")]
    Png,

    [Description(".gif")]
    Gif,

    [Description(".bmp")]
    Bmp
}