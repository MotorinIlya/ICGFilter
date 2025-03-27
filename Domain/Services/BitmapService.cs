using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ICGFilter.Domain.Services;

public class BitmapService
{
    public static WriteableBitmap CreateBitmap(int width, int height)
    {
        return new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Opaque);
    }
}