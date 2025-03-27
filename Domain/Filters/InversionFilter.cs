using System.Drawing;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;
using ICGFilter.Domain.Filters;

namespace ICGFilter.Domain.Filters;

public class InversionFilter : IImageFilter
{
    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        var ptr = (byte*)buf.Address.ToPointer();

        var invBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var invBuf = invBitmap.Lock();
        var invPtr = (byte*)invBuf.Address.ToPointer();

        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                byte inv = 255;
                var offset = y * buf.RowBytes + x * 4;
                (var r, var g, var b) = ColorService.GetColor(ptr, offset);
                ColorService.SetColor(invPtr, offset, 
                    (byte)(inv - r), (byte)(inv - g), (byte)(inv - b));
            }
        }
        return invBitmap;
    }
}