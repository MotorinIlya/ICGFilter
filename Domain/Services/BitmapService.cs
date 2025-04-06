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

    public static int GetOffset(int x, int y, int rowBytes) => y * rowBytes + x * 4;

    public unsafe static void FillBitmapWhite(WriteableBitmap bitmap)
    {
        using var buffer = bitmap.Lock();
        var ptr = (byte*)buffer.Address.ToPointer();
        for (int i = 0; i < buffer.Size.Width * buffer.Size.Height * 4; i += 4)
        {
            ptr[i] = 255;
            ptr[i + 1] = 255;
            ptr[i + 2] = 255;
            ptr[i + 3] = 255;
        }
    }

    public unsafe static (float[,], float[,], float[,]) ToMatrix(ILockedFramebuffer buffer)
    {
        var ptr = (byte*)buffer.Address.ToPointer();
        var rmatrix = new float[buffer.Size.Width, buffer.Size.Height];
        var gmatrix = new float[buffer.Size.Width, buffer.Size.Height];
        var bmatrix = new float[buffer.Size.Width, buffer.Size.Height];

        for (var x = 0; x < buffer.Size.Width; x++)
        {
            for (var y = 0; y < buffer.Size.Height; y++)
            {
                var offset = GetOffset(x, y, buffer.RowBytes);
                bmatrix[x,y] = ptr[offset];
                gmatrix[x,y] = ptr[offset + 1];
                rmatrix[x,y] = ptr[offset + 2];
            }
        }
        return (rmatrix, gmatrix, bmatrix);
    }
}