using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class BlackWhiteFilter : IImageFilter
{
    public unsafe WriteableBitmap Apply(WriteableBitmap bitmap)
    {
        using var buf = bitmap.Lock();
        var newBitmap = BitmapService.CreateBitmap(buf.Size.Width, buf.Size.Height);
        using var newBuf = newBitmap.Lock();

        var ptr = (byte*)buf.Address.ToPointer();
        var newPtr = (byte*)newBuf.Address.ToPointer();
        for (var x = 0; x < buf.Size.Width; x++)
        {
            for (var y = 0; y < buf.Size.Height; y++)
            {
                var offset = BitmapService.GetOffset(x, y, buf.RowBytes);
                var (r, g, b) = ColorService.GetColor(ptr, offset);
                var grayColor = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                ColorService.SetColor(newPtr, offset, grayColor, grayColor, grayColor);
            }
        }

        return newBitmap;
    }
}