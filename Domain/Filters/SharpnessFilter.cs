using System;
using Avalonia.Media.Imaging;
using ICGFilter.Domain.Services;

namespace ICGFilter.Domain.Filters;

public class SharpnessFitler : IImageFilter
{
    private static readonly int[,] _kernel =
    {
        {-1, -1, -1},
        {-1, 9, -1},
        {-1, -1, -1}
    };
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
                var r = 0;
                var g = 0;
                var b = 0;
                for (var kx = -1; kx <= 1; kx++)
                {
                    for (var ky = -1; ky <= 1; ky++)
                    {
                        var px = x + kx;
                        var py = y + ky;
                        if (px < 0 || py < 0 || px >= buf.Size.Width || py >= buf.Size.Height)
                        {
                            continue;
                        }

                        var offset = BitmapService.GetOffset(px, py, newBuf.RowBytes);
                        var kernalValue = _kernel[kx + 1, ky + 1];

                        r += ptr[offset + 2] * kernalValue;
                        g += ptr[offset + 1] * kernalValue;
                        b += ptr[offset] * kernalValue;
                    }
                }

                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);

                var origOffset = BitmapService.GetOffset(x, y, newBuf.RowBytes);
                ColorService.SetColor(newPtr, origOffset, (byte)r, (byte)g, (byte)b);
            }
        }
        return newBitmap;
    }
}